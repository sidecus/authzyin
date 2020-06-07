import * as React from 'react';
import { SampleClientContext } from '../store/store';

interface ISampleContextProps {
    data: SampleClientContext;
}

export const SampleContext = ({data}: ISampleContextProps) => {
    return (<div><pre>{JSON.stringify(data, null, 2)}</pre></div>);
}