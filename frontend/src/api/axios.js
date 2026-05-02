import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: 'http://localhost:5153/api',
});

export default axiosInstance;
