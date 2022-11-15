<template>
	<div class="card">
		<div class="card-header">
			<b>Thảo luận</b>
		</div>
		<div class="card-body">
			<form id="binhluan" :action="path + '/admin/api/addcomment'" method="POST" enctype="multipart/form-data">
				<input name="execution_id" :value="model.id" type="hidden" />
				<textarea required name="comment" style="padding:10px 20px;border: 1px solid #d5d5d5; width:100%;border-radius: 5px;" rows="2" placeholder="Bình luận?"></textarea>
				<div>
					<div class="float-right">
						<button class="btn btn-sm btn-gradient-primary text-light px-4 mb-0 add_comment">Bình luận</button>
					</div>
					<div class="d-inline-block">
						<input name="file[]" multiple type="file" class="dropify" data-height="75" data-max-file-size="10M" />
					</div>
				</div>
			</form>
			<hr />
			<ul class="list-unstyled" id="comment_box">
				<li class="media comment_box my-2" :data-id="comment.id" :data-read="comment.is_read" v-for="(comment,index) in comments">
					<img class="mr-3 rounded-circle" :src="path + comment.user.image_url" width="50" alt="">
					<div class="media-body border-bottom" style="display:grid;">
						<h5 class="mt-0 mb-1">{{comment.user.fullName}} <small class="text-muted"> - {{comment.created_at | moment('HH:mm DD/MM/YYYY')}}</small></h5>
						<div class="mb-2" style="white-space:pre-wrap">{{comment.comment}}</div>
						<div class="mb-2 attach_file file-box-content"></div>
					</div>
				</li>
			</ul>
			<div class="text-center load_more"><a href="#" class="btn btn-primary btn-sm px-5">Xem thêm bình luận</a></div>
		</div>
	</div>
</template>
<script>
	export default {
		components: {
		},
		props: {
			model: {
				type: Object,
				default: () => ({}),
			},
		},
		data() {
			return {
				comments: [],
				from_id: null,
				path: path
			};
		},
		mounted() {
			this.getComments();
			$(".dropify").dropify();
		},
		methods: {
			async getComments() {
				/// Lấy comments
				var execution_id = this.model.id;
				var ress = await $.ajax({
					url: path + "/admin/api/morecomment",
					data: { execution_id: execution_id },
				});
				this.comments = ress.comments;
			}
		}
	}
</script>

<style lang="scss"></style>
