import * as React from 'react';
import { makeStyles, FormControl, Radio, RadioGroup, FormControlLabel, Typography } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { useSampleAppBoundActionCreators } from '../store/actions';
import { barsSelector, currentBarSelector } from '../store/selectors';
import { Bar } from '../api/Api';

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
    const { enterBar } = useSampleAppBoundActionCreators();

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>, value: string) => {
        const selectedBarId = parseInt(value);
        enterBar(selectedBarId);
    }

    if (!bars) {
        return <></>;
    }

    return (
        <div>
            <Typography variant="h4" component="h2">
                Select one of the bars below:
            </Typography>
            <FormControl className={classes.formControl}>
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
            <Typography variant="h5" component="h3">
                {currentBar === -1 ? 'You are not in any bar.' : `You've walked into ${bars[currentBar].name}`}
            </Typography>
        </div>
    );
};