import repository from "../service/repository";

const resoure = "auth";
export default {
  logout() {
    return repository.post(`/v1/${resoure}/Logout`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
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
    });;
  },
  get(id) {
    return repository.get(`/v1/${resoure}/get/${id}`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
  TokenInfo(token) {
    return repository
      .get(`/v1/${resoure}/TokenInfo?token=${token}`)
      .then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
};
