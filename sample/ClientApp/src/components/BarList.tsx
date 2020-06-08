import * as React from 'react';
import { makeStyles, FormControl, Radio, RadioGroup, FormControlLabel, Typography } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { useSampleAppBoundActionCreators } from '../store/actions';
import { barsSelector, currentBarSelector, authZyinContextSelector } from '../store/selectors';
import { Bar } from '../api/Api';
import { useAuthorize } from '../authzyin/Authorize';
import { Severity } from '../store/store';

const useStyles = makeStyles((theme) => ({
    formControl: {
      margin: theme.spacing(1),
      minWidth: 120,
    },
    selectEmpty: {
      marginTop: theme.spacing(2),
    },
  }));

export const BarList = () => {
    const classes = useStyles();
    const bars = useSelector(barsSelector);
    const currentBar = useSelector(currentBarSelector);
    const { enterBar, setAlert, setCurrentBar } = useSampleAppBoundActionCreators();
    const authorize = useAuthorize(authZyinContextSelector);

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>, value: string) => {
        const selectedBarId = parseInt(value);

        // Authorize on client first
        setCurrentBar(-1);
        if (authorize("CanEnterBar", bars[selectedBarId])) {
            enterBar(selectedBarId);
        } else {
            setAlert({
                severity: Severity.Error,
                message: `Client authorizatio failed - not allowed to go to ${selectedBarId}:${bars[selectedBarId].name}`,
            });
        }
    }

    if (!bars || bars.length === 0) {
        return <></>;
    } else {
        return (
            <div>
                <FormControl className={classes.formControl}>
                    <Typography variant="h4" component="h2">
                        Local bars available:
                    </Typography>
                    <RadioGroup row aria-label="bars" name="bars" value={currentBar === -1 ? '' : `${currentBar}`} onChange={handleChange}>
                    {
                        bars.map((bar: Bar) => {
                            return (
                                <FormControlLabel key={bar.id} label={bar.name} value={`${bar.id}`} control={<Radio />} />
                            );
                        })
                    }
                    </RadioGroup>
                </FormControl>
            </div>
        );
    }
};