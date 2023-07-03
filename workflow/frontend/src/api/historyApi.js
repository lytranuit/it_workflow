import repository from "../service/repository";

const resoure = "history";
export default {
  table(params) {
    return repository
      .post(`/v1/${resoure}/table`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  export(params) {
    return repository
      .post(`/v1/${resoure}/exportexcel`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
};
