import * as React from 'react';
import { makeStyles, Card, CardHeader, CardContent, Switch, Typography, FormControlLabel } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { useSampleAppBoundActionCreators } from '../store/actions';
import { placesSelector, sneakInSelector } from '../store/selectors';
import { Place } from '../api/Contract';
import { AlertBanner } from './Alert';
import { PlaceComponent } from './PlaceComponent';

const useStyles = makeStyles((theme) => ({
    placelist: {
      display: 'flex',
      flexDirection: 'row',
      justifyContent: 'space-between',
    },
}));

export const PlaceList = () => {
    const classes = useStyles();
    const places = useSelector(placesSelector);
    const sneakIn = useSelector(sneakInSelector);
    const { setSneakIn, getPlaces } = useSampleAppBoundActionCreators();

    React.useEffect(() => {
        getPlaces();
    }, [getPlaces]);

    const handleSneakInChange = (event: React.ChangeEvent<HTMLInputElement>, checked: boolean) => {
        setSneakIn(checked);
    }

    if (places && places.length >= 0) {
        const placesInfo = places.map((place: Place) => (<PlaceComponent key={place.id} place={place} sneakIn={sneakIn} />));
    
        return (
            <Card variant="outlined">
                <CardHeader title='Local places nearby you' />
                <CardContent>
                    <Typography variant="subtitle1" component="div">Hover to see more details. Click to enter</Typography>
                    <div className={classes.placelist}>
                        {placesInfo}
                    </div>
                    <FormControlLabel
                        control={<Switch checked={sneakIn} onChange={handleSneakInChange} name="sneakIn" />}
                        label="Sneak in (bypass client authorization)"
                    />
                    <AlertBanner />
                </CardContent>
            </Card>
        );
    };

    return <></>;
};