import * as React from 'react';
import { makeStyles, FormControl, Radio, RadioGroup, FormControlLabel, Card, CardHeader, CardContent, Switch } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { useSampleAppBoundActionCreators } from '../store/actions';
import { barsSelector, currentBarSelector, authZyinContextSelector, sneakInSelector } from '../store/selectors';
import { Bar } from '../api/Api';
import { useAuthorize } from '../authzyin/Authorize';
import { Severity } from '../store/store';
import { LightTooltip } from './LightTooltip';
import { AlertBanner } from './Alert';

const useStyles = makeStyles((theme) => ({
    formControl: {
      minWidth: 240,
    },
  }));

export const BarList = () => {
    const classes = useStyles();
    const bars = useSelector(barsSelector);
    const currentBar = useSelector(currentBarSelector);
    const sneakIn = useSelector(sneakInSelector);
    const { setAlert, setSneakIn, setCurrentBar, enterBar } = useSampleAppBoundActionCreators();
    const authorize = useAuthorize(authZyinContextSelector);

    const handleBarChange = (event: React.ChangeEvent<HTMLInputElement>, value: string) => {
        const selectedBarId = parseInt(value);

        // Reset current bar
        setCurrentBar(-1);

        // Authorize on client
        if (sneakIn || authorize("CanEnterBar", bars[selectedBarId])) {
            // invoke server api
            enterBar(selectedBarId);
        } else {
            setAlert({
                severity: Severity.Error,
                message: `Client authorizatio failed - not allowed to go to ${selectedBarId}:${bars[selectedBarId].name}`,
            });
        }
    }

    const handleSneakInChange = (event: React.ChangeEvent<HTMLInputElement>, checked: boolean) => {
        setSneakIn(checked);
    }

    if (bars && bars.length >= 0) {
        const barInfo = bars.map((bar: Bar) => {
            return (
                <LightTooltip key={bar.id} title={`accepts ${bar.acceptedPaymentMethods[0].toString()}`} placement='top' arrow>
                    <FormControlLabel key={bar.id} label={bar.name} value={`${bar.id}`} control={<Radio />} />
                </LightTooltip>
            );
        });
    
        return (
            <Card variant="outlined">
                <CardHeader title='Local bars nearby you' />
                <CardContent>
                    <FormControl className={classes.formControl}>
                        <RadioGroup row aria-label="bars" name="bars" value={currentBar === -1 ? '' : `${currentBar}`} onChange={handleBarChange}>
                            {barInfo}
                        </RadioGroup>
                        <FormControlLabel
                            control={<Switch checked={sneakIn} onChange={handleSneakInChange} name="sneakIn" />}
                            label="Try to sneak in"
                        />
                        <AlertBanner />
                    </FormControl>
                </CardContent>
            </Card>
        );
    };

    return <></>;
};