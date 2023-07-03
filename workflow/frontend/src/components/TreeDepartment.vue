<template>
  <li class="dd-item" :data-id="node.id" :id="'menuItem_' + node.id">
    <div class="dd-handle">
      <div>
        <span
          class="showhide"
          v-if="node.children && node.children.length > 0 && !node.expanded"
          @click="node.expanded = !node.expanded"
          >-</span
        >
        <span
          class="showhide"
          v-else-if="node.children && node.children.length > 0 && node.expanded"
          @click="node.expanded = !node.expanded"
          >+</span
        >
        <a href="#" @click="edit(node)">{{ node.name }}</a>
      </div>
      <div class="dd-nodrag btn-group ml-auto">
        <button class="btn btn-sm btn-outline-light">
          <i class="far fa-edit"></i>
        </button>
        <button
          class="btn btn-sm btn-outline-light ml-2"
          @click="remove(node.id)"
        >
          <i class="far fa-trash-alt"></i>
        </button>
      </div>
    </div>
    <ol class="dd-list" v-show="!node.expanded">
      <TreeDepartment
        v-for="child in node.children"
        :key="child.id"
        :node="child"
        @edit="edit"
        @remove="remove"
      />
    </ol>
  </li>
</template>

<script setup>
const props = defineProps({
  node: {
    type: Object,
    required: true,
  },
});

const emit = defineEmits(["edit", "remove"]);
const edit = (n) => {
  emit("edit", n);
};
const remove = (id) => {
  emit("remove", id);
};
</script>
