import repository from "../service/repository";

const resoure = "user";
export default {
  get(id) {
    return repository.get(`/v1/${resoure}/get/${id}`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });
  },
  departments() {
    return repository.get(`/v1/${resoure}/departments`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });
  },
  roles() {
    return repository.get(`/v1/${resoure}/roles`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });
  },
  create(formData) {
    return repository
      .post(`/v1/${resoure}/create`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });
  },
  edit(formData) {
    return repository
      .post(`/v1/${resoure}/edit`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });
  },
  changepassword(formData) {
    return repository
      .post(`/v1/${resoure}/changepassword`, formData)
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });
  },
  excel() {
    return repository.post(`/v1/${resoure}/excel`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });
  },
  sync() {
    return repository.post(`/v1/${resoure}/sync`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });
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
      });
  },
  delete(id) {
    return repository
      .post(
        `/v1/${resoure}/delete`,
        { id: id },
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
      });
  },
};
