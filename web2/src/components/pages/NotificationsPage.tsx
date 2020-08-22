import React, { FC } from 'react';
import { Typography } from '@material-ui/core';
import { useSelector } from 'react-redux';
import NotificationsTable from '../tables/NotificationsTable';
import { selectNotifications } from '../../hooks/selectors';

const NotificationsPage: FC = () => {
  const { notifications } = useSelector(selectNotifications);
  const showTable = notifications.length > 0;

  return (
    <div>
      <Typography variant="h4">
        Notifications
      </Typography>
      <br />
      {
        showTable
          ? <NotificationsTable notifications={notifications} />
          : <Typography variant="body1">You do not have any notifications.</Typography>
      }
    </div>
  );
};

export default NotificationsPage;
