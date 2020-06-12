import * as React from 'react';
import { makeStyles, Typography, Button } from '@material-ui/core';
import { useSampleAppBoundActionCreators } from '../store/actions';
import { Place, IsAgeLimitedPlace, IsBar } from '../api/Contract';
import { Severity } from '../store/state';
import { LightTooltip } from './LightTooltip';
import LocalBarIcon from '@material-ui/icons/LocalBar';
import WeekendIcon from '@material-ui/icons/Weekend';
import { useAuthorize } from 'authzyin.js';

const useStyles = makeStyles((theme) => ({
    button: {
      margin: theme.spacing(1),
    },
}));

export const PlaceComponent = ({place, sneakIn}: {
    place: Place,
    sneakIn: boolean,
}) => {
    const classes = useStyles();
    const { setAlert, setCurrentPlace, enterPlace } = useSampleAppBoundActionCreators();
    const authorize = useAuthorize();
    const authorized = authorize(place.policy, place);

    const handlePlaceChange = () => {
        setCurrentPlace(-1);

        // Authorize on client
        if (sneakIn || authorized) {
            enterPlace(place.id);       // invoke server api if client authorization failed or we can bypass
        } else {
            setAlert({
                severity: Severity.Error,
                message: `Client authorizatio failed - not allowed to go to ${place.name}`,
            });
        }
    }

    return (
        <div>
            <LightTooltip key={place.id} placement='top' arrow
                title={
                    <React.Fragment>
                        <Typography color="inherit">{place.name}</Typography>
                        <Typography color="secondary">{`accepts ${place.acceptedPaymentMethods[0].toString()}`}</Typography>
                        <Typography color="secondary">{IsAgeLimitedPlace(place) && `Age:${place.minAge}-${place.maxAge}`}</Typography>
                        <Typography color="error">{!authorized && 'Not authorized'}</Typography>
                    </React.Fragment>
                }
            >
                <Button
                    variant="contained"
                    className={classes.button}
                    color = {authorized ? "primary" : "secondary"}
                    startIcon={IsBar(place) ? <LocalBarIcon/> : <WeekendIcon/>}
                    onClick={() => handlePlaceChange()}
                >
                    {place.name}
                </Button>
            </LightTooltip>
        </div>
    );
};