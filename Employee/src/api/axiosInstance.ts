import axios from 'axios';

export const axiosInstance = axios.create({
  headers: {
    common: {
      'Content-Type': 'application/json',
    },
  },
});
