import repository from "../service/repository";

const resoure = "department";
export default {
  get() {
    return repository.get(`/v1/${resoure}/get`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
  table(params) {
    return repository
      .post(`/v1/${resoure}/table`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
  remove(items) {
    return repository
      .post(`/v1/${resoure}/remove`, items, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
  save(items) {
    return repository
      .post(`/v1/${resoure}/save`, items, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
  saveorder(items) {
    return repository
      .post(
        `/v1/${resoure}/saveorder`,
        { data: items },
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
      .then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
};
