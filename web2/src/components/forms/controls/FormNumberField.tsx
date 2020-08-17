import React, { FC, ChangeEvent } from 'react';
import { FormControlLabel, TextField, useTheme } from '@material-ui/core';
import { useFormStyles } from '../../../styles/styles';

interface Props {
  value: number,
  label: string,
  onChanged: (e: ChangeEvent<HTMLInputElement>) => void,
  error?: boolean,
  helperText?: string
}

const FormNumberField: FC<Props> = ({
  value, label, onChanged, error, helperText,
}) => {
  const theme = useTheme();
  const styles = useFormStyles(theme);

  return (
    <FormControlLabel
      value={value}
      label={label}
      labelPlacement="start"
      className={styles.label}
      control={(
        <TextField
          className={styles.control}
          onChange={onChanged}
          error={error}
          helperText={helperText}
          type="number"
        />
      )}
    />
  );
};

export default FormNumberField;
