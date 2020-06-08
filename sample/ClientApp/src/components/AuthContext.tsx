import * as React from 'react';
import { SampleClientContext } from '../api/Api';
import { CardHeader, Card, CardContent } from '@material-ui/core';
import ReactJson from 'react-json-view';

interface IAuthContextProps {
    data: SampleClientContext;
}

export const AuthContext = ({data}: IAuthContextProps) => {
    return (
        <Card variant="outlined">
            <CardHeader title='Client authorization context' />
            <CardContent>
                <ReactJson src={data} theme='summerfruit' iconStyle='triangle' collapsed={2} enableClipboard={false}/>
            </CardContent>
        </Card>
    );
}