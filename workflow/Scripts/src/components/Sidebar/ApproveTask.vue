<template>
    <div class="container" id="approve">
        <ul class="nav nav-pills nav-justified mb-3" role="tablist">
            <li class="nav-item waves-effect waves-light" v-if="url != null && readonly == false">
                <a class="nav-link" data-toggle="tab" href="#suggest" role="tab" aria-selected="false">Ký tên</a>
            </li>
            <li class="nav-item waves-effect waves-light" v-if="url != null && readonly == true">
                <a class="nav-link" data-toggle="tab" href="#viewer" role="tab" aria-selected="false">Xem file</a>
            </li>
            <li class="nav-item waves-effect waves-light" v-for="(element,index) in blocks_approve">
                <a class="nav-link" data-toggle="tab" :href="'#tab-'+index" role="tab" aria-selected="false">{{element.label}}</a>
            </li>
        </ul>
        <div class="tab-content" style="height: calc(100% - 53px);">
            <div class="tab-pane h-100" id="suggest" role="tabpanel" v-if="url != null && readonly == false">
                <div class="card no-shadow border">
                    <div class="card-body">
                        <div class="row g-0">
                            <div class="col-9" style="border: 5px solid #d7d7d7;border-left: 0;">
                                <div id='pdf-viewer' style="height:620px;" :data-url="file"></div>
                                <!--<PDFViewer :source="url"
                                   style="height: 620px;/>-->
                            </div>
                            <div class="col-3 order-first" style="border: 5px solid #d7d7d7; height: 640px; display: inline-block; padding: 10px">
                                <div class="base-title">Chữ ký</div>
                                <span class="base-subtitle">Kéo chữ ký vào văn bản để ký</span>
                                <div id="sign">
                                    <div class="signature ui-draggable ui-draggable-handle" :data-id="sign.id" v-if="sign != null">
                                        <div class="d-inline-block">
                                            <img class="sign_image" :src="sign.image_sign" style="width:120px;height:auto;" alt="...">
                                            <div class="sign_info" style="align-self:center">
                                                <div>{{sign.fullName}}</div>
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
            <div class="tab-pane h-100" id="viewer" role="tabpanel" v-if="url != null && readonly == true">
                <embed :src="url" style="width: 100%;height: 100%;" type="application/pdf">
            </div>
            <div class="tab-pane h-100" :id="'tab-'+index" role="tabpanel" v-for="(element,index) in blocks_approve">
                <div class="bg-white py-3">
                    <FormTask :departments="departments" :users="users" :fields="element.fields" readonly></FormTask>
                </div>
            </div>
        </div>
    </div>

</template>

    
<script>
    import store from '../../../example/store';
    import FormTask from './FormTask';
    //import PDFViewer from 'pdf-viewer-vue'
    export default {
        inject: ['i18n'],
        components: {
            FormTask,
            //PDFViewer
        },
        props: {
            model: {
                type: Object,
                default: () => ({}),
            },
            users: {
                type: Array,
                default: () => ([]),
            },
            nodes: {
                type: Array,
                default: () => ([]),
            },
            departments: {
                type: Array,
                default: () => ([]),
            },
            readonly: {
                type: Boolean,
                default: () => (false),
            },
        },
        data() {
            return {
                blocks_approve: [],
                activeName: '1',
                thePdf: null,
                url: null,
                file: null,
                sign: null
            }
        },
        computed: {
            data_activity() {
                return store.state.data_activity;
            },
            current_user() {
                return store.state.current_user;
            }
        },
        mounted() {
            var model = this.model;
            var that = this;
            var indexBlock = this.nodes.findIndex(function (item) {
                return item.id == model.block_id;
            });
            var block = this.nodes[indexBlock];
            var blocks_approve_id = block.data_setting.blocks_approve_id || [];
            var blocks_approve = blocks_approve_id.map(function (block_id) {
                let indexActivity = that.data_activity.findLastIndex(function (i) {
                    return i.block_id == block_id;
                });
                let activity = that.data_activity[indexActivity];
                return activity;
            });
            this.blocks_approve = blocks_approve;
            if (blocks_approve.length)
                this.activeName = blocks_approve[0].id;
            //console.log(blocks_approve);

            var blocks_esign_id = block.data_setting.blocks_esign_id;
            var indexActivity = this.data_activity.findLastIndex(function (item) {
                return item.block_id == blocks_esign_id;
            });
            if (indexActivity != -1) {
                var activity_esign = this.data_activity[indexActivity];
                var esign = activity_esign.data_setting.esign || {};
                //console.log(esign);
                var files = esign.files || [];
                var url = path + files[files.length - 1].url;
                this.file = files[files.length - 1].url;
                this.url = url + "?token=sdfxvbdfgertewrkvcbgyrewbnfgsdfwetyrtrgdgweqfvqqazqhjkuiyort";

                if (!that.readonly) {
                    var pdfjsLib = window['pdfjs-dist/build/pdf'];
                    // The workerSrc property shall be specified.
                    pdfjsLib.GlobalWorkerOptions.workerSrc = '/lib/pdfview/pdf.worker.js';

                    // Asynchronous download of PDF
                    var loadingTask = pdfjsLib.getDocument(this.url);
                    loadingTask.promise.then(async function (pdf) {
                        await that.getsign();
                        that.thePdf = pdf;

                        var viewer = document.getElementById('pdf-viewer');
                        for (var page = 1; page <= pdf.numPages; page++) {
                            var canvas = document.createElement("canvas");
                            canvas.className = 'pdf-page-canvas';
                            var div = document.createElement("div");
                            div.className = "box-canvas";
                            div.appendChild(canvas);
                            viewer.appendChild(div);
                            await that.renderPage(page, canvas);
                        }
                        // Fetch the first page
                        $("#pdf-viewer").data("activity_esign", activity_esign.id);
                        $("#sign .signature").draggable({
                            stop: function (event, ui) {
                                if (ui.position.left < 0) {
                                    var user_id = $(ui.helper).data("id");
                                    $("#sign .disabled.signature[data-id='" + user_id + "']").remove();
                                    $(ui.helper).css({
                                        "top": 0 + "px",
                                        "left": 0 + "px"
                                    }).appendTo($("#sign"));
                                }
                            }
                        });

                        $("#sign .sign_image").resizable();
                        $('.box-canvas').droppable({
                            // only accept elements matching this CSS selector
                            accept: '.signature',
                            // Require a 100% element overlap for a drop to be possible
                            drop: function (event, ui) {
                                var user_id = $(ui.draggable).data("id");
                                $(ui.draggable).appendTo($(this));
                                var newPosX = ui.offset.left - $(this).offset().left;
                                var newPosY = ui.offset.top - $(this).offset().top + this.scrollTop;
                                $(ui.draggable).css({
                                    "top": newPosY + "px",
                                    "left": newPosX + "px"
                                });
                                //console.log(newPosX)
                                if (newPosX < 0) {
                                    $(ui.draggable).css({
                                        "top": 0 + "px",
                                        "left": 0 + "px"
                                    }).appendTo($("#sign"));
                                } else {
                                    var sign_in_init = $("#sign .signature[data-id='" + user_id + "']")
                                    if (sign_in_init.length) { // có
                                        sign_in_init.addClass("disabled");
                                    } else {
                                        sign_in_init = $(ui.draggable).clone();
                                        sign_in_init.addClass("disabled").css({
                                            "top": 0 + "px",
                                            "left": 0 + "px"
                                        });
                                        sign_in_init.appendTo($("#sign"));
                                    }
                                }
                            }
                        });

                    }, function (reason) {
                        // PDF loading error
                        console.error(reason);
                    });
                }
                setTimeout(function () {
                    if (that.readonly) {
                        console.log($("#viewer"))
                        console.log($("[href='#viewer']"))
                        $("#viewer").addClass("active");
                        $("[href='#viewer']").addClass("active");
                    } else {
                        $("#suggest").addClass("active");
                        $("[href='#suggest']").addClass("active");
                    }
                }, 100)

            } else {
                setTimeout(function () {
                    $("#tab-0").addClass("active");
                    $("[href='#tab-0']").addClass("active");
                }, 100);
            }

        },
        methods: {
            async renderPage(pageNumber, canvas) {
                return this.thePdf.getPage(pageNumber).then(function (page) {
                    var viewport = page.getViewport({
                        scale: 1
                    });
                    canvas.height = viewport.height;
                    canvas.width = viewport.width;
                    page.render({
                        canvasContext: canvas.getContext('2d'),
                        viewport: viewport
                    });
                });
            },
            async getsign() {
                var email = this.current_user.email;
                var sign = await $.ajax({
                    url: "https://esign.pymepharco.com/api/userinfo?email=" + email,
                    dataType: "JSON",
                    type: "GET",
                });

                if (!sign.is_sign) {
                    alert("Bạn chưa có chữ ký điện tử. Vui lòng liên hệ Phòng IT!");
                    return
                }
                sign.image_sign = "https://esign.pymepharco.com" + sign.image_sign + "?token=sdfxvbdfgertewrkvcbgyrewbnfgsdfwetyrtrgdgweqfvqqazqhjkuiyort";
                this.sign = sign;

            }
        }
    }
</script>
<style lang="scss" scoped>
    #approve {
        height: 100%;
        overflow: auto;
    }

    .sign_image {
        background: transparent
    }
</style>
