import * as React from 'react';
import { Alert } from '@material-ui/lab';
import { useSelector } from 'react-redux';
import { alertSelector } from '../store/selectors';

export const AlertBanner = () => {
    const alert = useSelector(alertSelector);

    if (alert.message) {
        return (<Alert severity = {alert.severity}>{alert.message}</Alert>);
    } else {
        return <></>;
    }
}