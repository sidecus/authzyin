import * as React from 'react';
import { SampleClientContext } from '../api/Api';
import { Typography } from '@material-ui/core';

interface ISampleContextProps {
    data: SampleClientContext;
}

export const SampleContext = ({data}: ISampleContextProps) => {
    return (
        <div>
            <Typography variant="h4" component="h2">
                Client context JSON:
            </Typography>
            <pre>{JSON.stringify(data, null, 2)}</pre>
        </div>
    );
}