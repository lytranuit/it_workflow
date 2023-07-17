import moment from "moment/moment";
const formatSize = (bytes) => {
  if (bytes === 0) {
    return "0 B";
  }

  let k = 1000,
    dm = 3,
    sizes = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"],
    i = Math.floor(Math.log(bytes) / Math.log(k));

  return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i];
};
const formatTime = (sec) => {
  let formatted = moment.utc(sec * 1000).format("HH:mm:ss");
  if (sec < 3600) {
    formatted = moment.utc(sec * 1000).format("mm:ss");
  }
  return formatted;
};
const formatPrice = (value) => {
  let val = (value / 1).toFixed(2).replace(",", ".");
  return val.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
};
const formatDate = (value, fomat = "YYYY-MM-DD") => {
  return moment(value).format(fomat);
};

const printTrigger = (link) => {
  $("body").append(
    '<iframe class="iFramePdf" src="' +
      link +
      '" style="display:none;"></iframe>'
  );
  var getMyFrame = $(".iFramePdf").last()[0];
  console.log(getMyFrame);
  getMyFrame.focus();
  getMyFrame.contentWindow.print();
};
export { formatSize, formatTime, formatPrice, formatDate, printTrigger };
