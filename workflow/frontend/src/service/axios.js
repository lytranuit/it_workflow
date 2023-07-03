import axios from "axios";
export function useAxios() {
  const axiosinstance = axios.create({
    baseURL: import.meta.env.VITE_BASEURL,
  });
  return { axiosinstance };
}
