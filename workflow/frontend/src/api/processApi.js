import repository from "../service/repository";

const resoure = "process";
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

  export(id) {
    return repository
      .post(
        `/v1/${resoure}/export`,
        { process_id: id },
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

  exportVersion(id) {
    return repository
      .post(
        `/v1/${resoure}/exportVersion`,
        { process_version_id: id },
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

  release(id) {
    return repository
      .post(
        `/v1/${resoure}/release`,
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
  
  unrelease(id) {
    return repository
      .post(
        `/v1/${resoure}/unrelease`,
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
