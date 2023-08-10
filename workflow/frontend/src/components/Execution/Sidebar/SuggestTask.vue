<template>
  <div class="" id="suggest">
    <div class="h-100" v-if="url != null">
      <div
        class="text-center mb-2"
        v-if="!model.blocking && Object.keys(signatures).length > 0"
      >
        <a href="#" class="btn-next" @click="save($event)"> Lưu lại </a>
      </div>

      <div class="card no-shadow border">
        <div class="card-body">
          <div class="row g-0">
            <div
              class="col-9"
              style="border: 5px solid #d7d7d7; border-left: 0"
            >
              <div id="pdf-viewer" style="height: 620px" :data-url="file"></div>
              <!--<PDFViewer :source="url"
                        style="height: 620px;/>-->
            </div>
            <div
              class="col-3 order-first"
              style="
                border: 5px solid #d7d7d7;
                height: 640px;
                display: inline-block;
                padding: 10px;
              "
            >
              <div class="base-title">Chữ ký</div>
              <span class="base-subtitle">Kéo chữ ký vào văn bản để ký</span>
              <div id="sign">
                <div
                  class="signature ui-draggable ui-draggable-handle"
                  :data-id="signature.block_id"
                  v-for="(signature, index) in signatures"
                >
                  <div class="d-inline-block">
                    <img
                      class="sign_image"
                      :src="signature.image"
                      style="width: 90px; height: auto"
                      alt="..."
                    />
                    <div class="sign_info" style="align-self: center">
                      <div>{{ signature.name }}</div>
                      <div>Current Time</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Api from "../../../api/Api";
import { useProcess } from "../../../stores/process";
var store = useProcess();
var thePdf = null;
//import PDFViewer from 'pdf-viewer-vue'
export default {
  components: {},
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
    nodes: {
      type: Array,
      default: () => [],
    },
    readonly: {
      type: Boolean,
      default: () => false,
    },
  },
  data() {
    return {
      // thePdf: null,
      url: null,
      file: null,
      sign: null,
    };
  },
  computed: {
    data_activity() {
      return store.data_activity;
    },
    //data_custom_block() {
    //    return store.state.data_custom_block;
    //},
    signatures() {
      var model = this.model;
      var block_id = model.block_id;
      //var esign = this.model.data_setting.esign || {};
      var indexBlock = this.nodes.findIndex(function (item) {
        return item.id == block_id;
      });
      var block = this.nodes[indexBlock];
      var blocks_esign_id = block.data_setting.blocks_esign_id;
      var blocks_approve = this.nodes.filter(function (item) {
        return (
          item.data_setting.blocks_esign_id == blocks_esign_id &&
          item.clazz == "approveTask"
        );
      });
      var signatures = {};
      //var data_custom_block = this.data_custom_block;
      for (var block of blocks_approve) {
        var activity = this.data_activity.filter(function (item) {
          return item.block_id == block.id;
        });
        if (activity.length > 0 && activity[0].executed) continue;
        var signnature = {
          block_id: block.id,
          name: "Block: " + block.label,
          image: "/images/tick.png",
        };
        signatures[block.id] = signnature;
      }
      return signatures;
    },
  },
  mounted() {
    $(document).on("click", "#sign .signature.disabled", function (e) {
      var user_id = $(this).data("id");
      var sign = $("#pdf-viewer .signature[data-id='" + user_id + "']");
      $("#pdf-viewer").animate(
        {
          scrollTop: sign.parent()[0].offsetTop + sign[0].offsetTop - 20,
        },
        "slow"
      );
    });
    var model = this.model;
    var that = this;
    //console.log(blocks_approve);

    var indexBlock = this.nodes.findIndex(function (item) {
      return item.id == model.block_id;
    });
    var block = this.nodes[indexBlock];
    var blocks_esign_id = block.data_setting.blocks_esign_id;
    var indexActivity = this.data_activity.findLastIndex(function (item) {
      return item.block_id == blocks_esign_id;
    });
    if (indexActivity != -1 && Object.keys(this.signatures).length > 0) {
      var activity_esign = this.data_activity[indexActivity];
      var esign = activity_esign.data_setting.esign || {};
      //console.log(esign);
      var files = esign.files || [];
      var url = files[files.length - 1].url;
      this.file = files[files.length - 1].url;
      this.url =
        url +
        "?token=sdfxvbdfgertewrkvcbgyrewbnfgsdfwetyrtrgdgweqfvqqazqhjkuiyort";

      var pdfjsLib = window["pdfjs-dist/build/pdf"];
      // console.log(pdfjsLib);
      // The workerSrc property shall be specified.
      pdfjsLib.GlobalWorkerOptions.workerSrc = "/lib/pdfview/pdf.worker.js";
      // Asynchronous download of PDF
      var loadingTask = pdfjsLib.getDocument(this.url);
      loadingTask.promise.then(
        async function (pdf) {
          // console.log(pdf);
          //await that.getsign();
          thePdf = pdf;

          var viewer = document.getElementById("pdf-viewer");
          for (var page = 1; page <= pdf.numPages; page++) {
            var canvas = document.createElement("canvas");
            canvas.className = "pdf-page-canvas";
            var div = document.createElement("div");
            div.className = "box-canvas";
            div.appendChild(canvas);
            viewer.appendChild(div);
            await that.renderPage(page, canvas);
          }
          ///FIND SUGGEST

          var suggests = model.data_setting.suggests;
          //console.log(suggest);
          if (suggests) {
            $("#sign .signature").each(function () {
              let block_id = $(this).data("id");
              var suggest = suggests[block_id];
              if (!suggest) return;
              let parent = $(".box-canvas:eq(" + (suggest.page - 1) + ")");
              var sign = $(this).clone();
              parent.append(sign);
              var height_page = $(".pdf-page-canvas", parent).height();
              $("#pdf-viewer .signature[data-id='" + block_id + "']")
                .css({
                  left: suggest.position_x + "px",
                  top:
                    height_page -
                    (suggest.position_y + suggest.image_size_height + 40) +
                    "px",
                })
                .draggable({
                  stop: function (event, ui) {
                    if (ui.position.left < 0) {
                      var user_id = $(ui.helper).data("id");
                      $(
                        "#sign .disabled.signature[data-id='" + user_id + "']"
                      ).remove();
                      $(ui.helper)
                        .css({
                          top: 0 + "px",
                          left: 0 + "px",
                        })
                        .appendTo($("#sign"));
                    }
                  },
                });
              $(
                "#pdf-viewer .signature[data-id='" + block_id + "'] .sign_image"
              ).resizable();
              $(this).addClass("disabled");
            });
          }
          // Fetch the first page

          $("#pdf-viewer").data("activity_esign", activity_esign.id);
          $("#sign .signature")
            .not(".disabled")
            .draggable({
              stop: function (event, ui) {
                if (ui.position.left < 0) {
                  var user_id = $(ui.helper).data("id");
                  $(
                    "#sign .disabled.signature[data-id='" + user_id + "']"
                  ).remove();
                  $(ui.helper)
                    .css({
                      top: 0 + "px",
                      left: 0 + "px",
                    })
                    .appendTo($("#sign"));
                }
              },
            });

          $("#sign .sign_image").not(".disabled").resizable();
          $(".box-canvas").droppable({
            // only accept elements matching this CSS selector
            accept: ".signature",
            // Require a 100% element overlap for a drop to be possible
            drop: function (event, ui) {
              var user_id = $(ui.draggable).data("id");
              $(ui.draggable).appendTo($(this));
              var newPosX = ui.offset.left - $(this).offset().left;
              var newPosY =
                ui.offset.top - $(this).offset().top + this.scrollTop;
              $(ui.draggable).css({
                top: newPosY + "px",
                left: newPosX + "px",
              });
              //console.log(newPosX)
              if (newPosX < 0) {
                $(ui.draggable)
                  .css({
                    top: 0 + "px",
                    left: 0 + "px",
                  })
                  .appendTo($("#sign"));
              } else {
                var sign_in_init = $(
                  "#sign .signature[data-id='" + user_id + "']"
                );
                if (sign_in_init.length) {
                  // có
                  sign_in_init.addClass("disabled");
                } else {
                  sign_in_init = $(ui.draggable).clone();
                  sign_in_init.addClass("disabled").css({
                    top: 0 + "px",
                    left: 0 + "px",
                  });
                  sign_in_init.appendTo($("#sign"));
                }
              }
            },
          });
        },
        function (reason) {
          // PDF loading error
          console.error(reason);
        }
      );
    }
  },
  methods: {
    async renderPage(pageNumber, canvas) {
      // console.log(this.thePdf);
      return thePdf.getPage(pageNumber).then(function (page) {
        var viewport = page.getViewport({
          scale: 1,
        });
        canvas.height = viewport.height;
        canvas.width = viewport.width;
        page.render({
          canvasContext: canvas.getContext("2d"),
          viewport: viewport,
        });
      });
    },
    async save(e) {
      e.preventDefault();
      var that = this;
      var model = this.model;
      if ($("#suggest").length) {
        var list = model.data_setting.suggests || {};
        if (
          $("#pdf-viewer .signature").length != $("#sign .signature").length
        ) {
          alert("Kéo chữ ký vào văn bản để gợi ý!");
          return;
        }
        $("#pdf-viewer .signature").each(function () {
          let sign = $(this);
          var sign_x = sign[0].offsetLeft;
          var sign_y = sign[0].offsetTop;
          var parent = sign.closest(".box-canvas");
          var page = parent.index() + 1;
          var height_page = $(".pdf-page-canvas", parent).height();
          var sign_image = $(".sign_image", sign);
          var sign_info = $(".sign_info", sign);
          //var sign_info_x = sign_info[0].offsetLeft;
          //var sign_info_y = sign_info[0].offsetTop;
          var sign_image_x = sign_image[0].offsetLeft;
          var sign_image_y = sign_image[0].offsetTop;
          var image_size_width = sign_image.width();
          var image_size_height = sign_image.height();
          var position_image_x = sign_image_x + sign_x;
          var position_image_y = height_page - (image_size_height + sign_y);
          var position_x = sign_x;
          var position_y = height_page - (image_size_height + sign_y + 40);
          if (!sign_info.length) {
            position_y = position_image_y;
          }
          var block_id = sign.data("id");

          var item = {
            page: page,
            position_x: position_x,
            position_y: position_y,
            position_image_x: position_image_x,
            position_image_y: position_image_y,
            image_size_width: image_size_width,
            image_size_height: image_size_height,
            block_id: block_id,
          };
          list[block_id] = item;
        });

        model.data_setting.suggests = list;
        var resp = await Api.updateactivity(model);
        location.reload();
      }
      return;
    },
  },
};
</script>
<style lang="scss" scoped>
#suggest {
  height: 100%;
  overflow: auto;
}

.sign_image {
  background: transparent;
}

.btn-next {
  display: inline-block;
  border-radius: 4px;
  padding: 6px 16px;
  background-color: #0c9cdd;
  color: #fff;
  border: 1px solid #0c9cdd;
  cursor: pointer;
  font-weight: 700;
}
</style>
