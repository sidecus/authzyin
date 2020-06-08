import * as React from 'react';
import { SampleClientContext } from '../api/Api';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { ExpansionPanel, ExpansionPanelSummary, ExpansionPanelDetails, Typography } from '@material-ui/core';

interface ISampleContextProps {
    data: SampleClientContext;
}

export const SampleContext = ({data}: ISampleContextProps) => {
    return (
        <ExpansionPanel>
            <ExpansionPanelSummary
                expandIcon={<ExpandMoreIcon />}
                aria-controls="panel1a-content"
                id="panel1a-header"
            >
                <Typography component='h4' color="textPrimary">View/hide client context JSON</Typography>
            </ExpansionPanelSummary>
            <ExpansionPanelDetails >
                <pre>{JSON.stringify(data, null, 2)}</pre>
            </ExpansionPanelDetails>        
        </ExpansionPanel>
    );
}