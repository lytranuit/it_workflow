<template>
  <div style="width: 100%">
    <draggable class="" v-model="model.fields">
      <transition-group type="transition" name="flip-list">
        <div
          class="item-control flex-m"
          v-for="(element, index) in model.fields"
          :key="element.id"
        >
          <div class="handle icon-move">
            <i class="fas fa-grip-vertical"></i>
          </div>
          <div
            class="control-preview-item background-sortable mb-2 ml-2 inline-flex-m"
          >
            <div class="form-input-data-preview mr-3 w-100">
              <div class="form-input-control flex-m">
                <div class="mb-2 custom-title-inline pr-2" style="width: 120px">
                  <div class="container-label">
                    <div class="container-left">
                      <div
                        id="idControlName"
                        class="font-weight-bold font-14 pr-2 d-inline-block"
                      >
                        <div style="">
                          {{ element.name }}
                          <span
                            class="text-danger"
                            style="float: inherit"
                            v-if="element.is_require"
                          >
                            *
                          </span>
                        </div>
                      </div>
                      <div
                        class="btn-selectTion-guide icon icon-infor-blue cursor-pointer icon-control"
                        style="color: #0c9cdd; float: right"
                        v-show="element.guide && element.guide != ''"
                      >
                        <div
                          class="con-ms-tooltip"
                          data-toggle="tooltip"
                          data-placement="top"
                          :data-original-title="element.guide"
                        >
                          <i class="fas fa-info-circle"></i>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
                <div
                  class="form-input-control-left w-100"
                  v-if="element.type != 'table'"
                >
                  <div v-if="element.type == 'number'">
                    <input
                      class="form-control form-control-sm number"
                      type="number"
                      :value="
                        element.has_default
                          ? element.data_setting.default_value
                          : ''
                      "
                    />
                  </div>
                  <div v-if="element.type == 'formular'">
                    <input
                      class="form-control form-control-sm"
                      type="number"
                      readonly
                    />
                  </div>
                  <div v-if="element.type == 'currency'">
                    <input
                      class="form-control form-control-sm number"
                      type="number"
                      :value="
                        element.has_default
                          ? element.data_setting.default_value
                          : ''
                      "
                    />
                  </div>
                  <div v-if="element.type == 'text'">
                    <input
                      class="form-control form-control-sm text"
                      type="text"
                      :value="
                        element.has_default
                          ? element.data_setting.default_value
                          : ''
                      "
                    />
                  </div>
                  <div v-if="element.type == 'email'">
                    <input
                      class="form-control form-control-sm email"
                      type="email"
                      :value="
                        element.has_default
                          ? element.data_setting.default_value
                          : ''
                      "
                    />
                  </div>
                  <div v-if="element.type == 'file'">
                    <input
                      class="form-control form-control-sm file"
                      type="file"
                    />
                  </div>

                  <div v-if="element.type == 'file_multiple'">
                    <input
                      class="form-control form-control-sm file"
                      type="file"
                      multiple
                    />
                  </div>
                  <div v-if="element.type == 'date'">
                    <Calendar dateFormat="yy/mm/dd" />
                  </div>
                  <div v-if="element.type == 'date_month'">
                    <Calendar dateFormat="yy-mm" view="month" />
                  </div>

                  <div v-if="element.type == 'date_time'">
                    <Calendar
                      dateFormat="yy-mm-dd HH:ii:ss"
                      showTime
                      hourFormat="24"
                    />
                  </div>

                  <div v-if="element.type == 'select'">
                    <TreeSelect
                      :options="get_options(element.data_setting.options)"
                      :value="
                        element.has_default
                          ? element.data_setting.default_value
                          : ''
                      "
                    >
                    </TreeSelect>
                  </div>

                  <div v-if="element.type == 'select_multiple'">
                    <TreeSelect
                      :options="get_options(element.data_setting.options)"
                      :value="
                        element.has_default
                          ? element.data_setting.default_value_array
                          : []
                      "
                      multiple
                    ></TreeSelect>
                  </div>

                  <div v-if="element.type == 'radio'">
                    <div
                      class="radio radio-success radio-circle"
                      v-for="option in element.data_setting.options"
                      :key="option.id"
                    >
                      <input
                        :id="'radio-' + option.id"
                        type="radio"
                        :name="'radio-' + element.id"
                      />
                      <label :for="'radio-' + option.id">
                        {{ option.name }}
                      </label>
                    </div>
                  </div>

                  <div v-if="element.type == 'checkbox'">
                    <div
                      class="checkbox checkbox-success"
                      v-for="option in element.data_setting.options"
                      :key="option.id"
                    >
                      <input :id="'checkbox-' + option.id" type="checkbox" />
                      <label :for="'checkbox-' + option.id">
                        {{ option.name }}
                      </label>
                    </div>
                  </div>
                  <div v-if="element.type == 'yesno'">
                    <div class="custom-control custom-switch switch-success">
                      <input
                        type="checkbox"
                        class="custom-control-input"
                        :id="'customSwitch_' + element.id"
                        checked=""
                      />
                      <label
                        class="custom-control-label"
                        :for="'customSwitch_' + element.id"
                      ></label>
                    </div>
                  </div>
                  <div v-if="element.type == 'textarea'">
                    <textarea
                      class="form-control form-control-sm textarea"
                      :value="
                        element.has_default
                          ? element.data_setting.default_value
                          : ''
                      "
                    ></textarea>
                  </div>

                  <div v-if="element.type == 'employee'">
                    <UserTreeSelect
                      :modelValue="
                        element.has_default
                          ? element.data_setting.default_value
                          : ''
                      "
                    ></UserTreeSelect>
                  </div>

                  <div v-if="element.type == 'employee_multiple'">
                    <UserTreeSelect
                      :modelValue="
                        element.has_default
                          ? element.data_setting.default_value_array
                          : []
                      "
                      multiple
                    ></UserTreeSelect>
                  </div>

                  <div v-if="element.type == 'department'">
                    <DepartmentTreeSelect
                      :modelValue="
                        element.has_default
                          ? element.data_setting.default_value
                          : ''
                      "
                    ></DepartmentTreeSelect>
                  </div>

                  <div v-if="element.type == 'department_multiple'">
                    <DepartmentTreeSelect
                      :modelValue="
                        element.has_default
                          ? element.data_setting.default_value_array
                          : []
                      "
                      multiple
                    ></DepartmentTreeSelect>
                  </div>

                  <div v-if="element.type == 'task'">
                    <div
                      class="checkbox checkbox-success checkbox-circle"
                      v-for="option in element.data_setting.options"
                      :key="option.id"
                    >
                      <input :id="'task-' + option.id" type="checkbox" />
                      <label :for="'task-' + option.id">
                        {{ option.name }}
                      </label>
                    </div>
                  </div>
                </div>
              </div>
              <div v-if="element.type == 'table'" style="overflow: auto">
                <table
                  class="table table-bordered mb-0 bg-white"
                  style="outline: 1px solid #dee2e6 !important"
                >
                  <thead class="">
                    <tr>
                      <th
                        class="border-top-0"
                        v-for="(column, index1) in element.data_setting.columns"
                        :key="column.id"
                      >
                        {{ column.name }}
                        <span class="text-danger" v-if="column.is_require"
                          >*</span
                        >
                      </th>
                    </tr>
                  </thead>
                  <tr>
                    <td
                      v-for="(column, index1) in element.data_setting.columns"
                      :key="column.id"
                    >
                      <div v-if="column.type == 'stt'">1</div>
                      <div v-if="column.type == 'number'">
                        <input
                          class="form-control form-control-sm number"
                          type="number"
                        />
                      </div>
                      <div v-if="column.type == 'currency'">
                        <input
                          class="form-control form-control-sm number"
                          type="number"
                        />
                      </div>
                      <div v-if="column.type == 'text'">
                        <input
                          class="form-control form-control-sm text"
                          type="text"
                        />
                      </div>
                      <div v-if="column.type == 'email'">
                        <input
                          class="form-control form-control-sm email"
                          type="email"
                        />
                      </div>
                      <div v-if="column.type == 'date'">
                        <input
                          class="form-control form-control-sm date"
                          type="text"
                        />
                      </div>
                      <div v-if="column.type == 'date_month'">
                        <input
                          class="form-control form-control-sm date_month"
                          type="text"
                        />
                      </div>

                      <div v-if="column.type == 'date_time'">
                        <input
                          class="form-control form-control-sm date_time"
                          type="text"
                        />
                      </div>
                      <div v-if="column.type == 'formular'">
                        <input
                          class="form-control form-control-sm number"
                          type="number"
                          readonly
                        />
                      </div>
                      <div v-if="column.type == 'textarea'">
                        <textarea
                          class="form-control form-control-sm textarea"
                        ></textarea>
                      </div>
                      <div v-if="column.type == 'yesno'">
                        <div
                          class="custom-control custom-switch switch-success"
                        >
                          <input
                            type="checkbox"
                            class="custom-control-input"
                            :id="'customSwitch_' + column.id + '_' + index1"
                            checked=""
                          />
                          <label
                            class="custom-control-label"
                            :for="'customSwitch_' + column.id + '_' + index1"
                          ></label>
                        </div>
                      </div>
                    </td>
                  </tr>
                  <tbody></tbody>
                </table>
              </div>
            </div>
            <div class="group-icon-right">
              <div class="dropdown">
                <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                  <i class="fas fa-ellipsis-v"></i>
                </a>
                <div class="dropdown-menu">
                  <a class="dropdown-item" href="#" v-on:click="edit(element)"
                    >Chỉnh sửa</a
                  >
                  <a class="dropdown-item" href="#" v-on:click="copy(element)"
                    >Nhân bản</a
                  >
                  <a
                    class="dropdown-item text-danger"
                    href="#"
                    v-on:click="remove_field(element.id)"
                    >Xóa</a
                  >
                </div>
              </div>
            </div>
          </div>
        </div>
      </transition-group>
    </draggable>
    <div class="dropdown text-center mt-3">
      <button
        class="btn btn-success btn-sm dropdown-toggle"
        type="button"
        id="dropdownMenuButton"
        data-toggle="dropdown"
        aria-haspopup="true"
        aria-expanded="false"
      >
        <i class="fas fa-plus mr-1"></i>
        Thêm trường
      </button>
      <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
        <a
          href="#"
          class="dropdown-item d-flex"
          @click="add_field(element.type, element.name)"
          v-for="element in myArray"
          :key="element.type"
          ><span class="mr-2" v-html="element.icon"></span>
          <div class="name-control">{{ element.name }}</div></a
        >
      </div>
    </div>
    <Dialog
      v-model:visible="visible"
      modal
      :header="temp_add.description"
      :style="{ width: '50vw' }"
    >
      <div class="form-group row">
        <div class="col-lg-12 mt-2">
          <b class="col-form-label"
            >Tên trường:<span class="text-danger">*</span></b
          >
          <div class="pt-1">
            <input
              class="form-control form-control-sm"
              type="text"
              name="name"
              required=""
              v-model="temp_add.name"
            />
          </div>
        </div>
        <div class="col-lg-12 mt-2" v-if="temp_add.type == 'currency'">
          <b class="col-form-label"
            >Tiền tệ:<span class="text-danger">*</span></b
          >
          <div class="pt-1">
            <select
              class="form-control form-control-sm"
              v-model="temp_add.data_setting.currency"
              name="currency"
              required=""
            >
              <option value="VND" selected>VND</option>
              <option value="USD">DOLLAR</option>
              <option value="EUR">EURO</option>
            </select>
          </div>
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="
            temp_add.type == 'date' ||
            temp_add.type == 'date_time' ||
            temp_add.type == 'date_month'
          "
        >
          <b class="col-form-label">Kiểu dữ liệu:</b>
          <div class="pt-1">
            <select
              class="form-control form-control-sm"
              v-model="temp_add.type"
            >
              <option value="date">Ngày/Tháng/Năm</option>
              <option value="date_month">Tháng/Năm</option>
              <option value="date_time">Ngày/Tháng/Năm Giờ:Phút</option>
            </select>
          </div>
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="
            temp_add.type == 'select' ||
            temp_add.type == 'select_multiple' ||
            temp_add.type == 'radio' ||
            temp_add.type == 'checkbox' ||
            temp_add.type == 'task'
          "
        >
          <b class="col-form-label">Danh sách giá trị:</b>
          <div class="list-group-item">
            <draggable class="" v-model="temp_add.data_setting.options">
              <transition-group type="transition" name="flip-list">
                <div
                  class="flex-m mb-2"
                  v-for="(option, index) in temp_add.data_setting.options"
                  :key="option.id"
                >
                  <div class="handle icon-move mr-2" style="cursor: move">
                    <i class="fas fa-grip-vertical"></i>
                  </div>
                  <input
                    class="form-control form-control-sm"
                    v-model="option.name"
                  />
                  <div
                    class="ml-2 text-danger"
                    style="cursor: pointer"
                    v-on:click="remove_option(index)"
                    v-if="temp_add.data_setting.options.length > 1"
                  >
                    <i class="fas fa-trash-alt"></i>
                  </div>
                </div>
              </transition-group>
            </draggable>

            <div class="btn-group" role="group" aria-label="Basic example">
              <button
                class="btn btn-secondary btn-sm"
                v-on:click="add_option($event)"
              >
                <i class="fas fa-plus mr-2"></i> Thêm dòng
              </button>
            </div>
          </div>
        </div>
        <div
          class="col-lg-12 mt-2 flex-m"
          v-if="
            temp_add.type == 'select' ||
            temp_add.type == 'select_multiple' ||
            temp_add.type == 'radio' ||
            temp_add.type == 'checkbox'
          "
        >
          <b class="col-form-label mr-3">Kiểu chọn giá trị:</b>
          <div>
            <div>
              <div class="radio radio-primary form-check-inline">
                <input
                  type="radio"
                  id="inlineRadio1"
                  value="select"
                  name="radioInline"
                  v-model="temp_add.type"
                />
                <label class="mb-0" for="inlineRadio1"> Sổ chọn một </label>
              </div>
              <div class="radio radio-primary form-check-inline">
                <input
                  type="radio"
                  id="inlineRadio2"
                  value="select_multiple"
                  name="radioInline"
                  v-model="temp_add.type"
                />
                <label class="mb-0" for="inlineRadio2"> Sổ chọn nhiều </label>
              </div>
            </div>
            <div>
              <div class="radio radio-primary form-check-inline">
                <input
                  type="radio"
                  id="inlineRadio3"
                  value="radio"
                  name="radioInline"
                  v-model="temp_add.type"
                />
                <label class="mb-0" for="inlineRadio3"> Tích chọn một </label>
              </div>
              <div class="radio radio-primary form-check-inline">
                <input
                  type="radio"
                  id="inlineRadio4"
                  value="checkbox"
                  name="radioInline"
                  v-model="temp_add.type"
                />
                <label class="mb-0" for="inlineRadio4"> Tích chọn nhiều </label>
              </div>
            </div>
          </div>
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="
            temp_add.type == 'employee' || temp_add.type == 'employee_multiple'
          "
        >
          <b class="col-form-label mr-3">Kiểu chọn giá trị:</b>

          <div class="radio radio-primary form-check-inline">
            <input
              type="radio"
              id="inlineRadio1"
              value="employee"
              name="radioInline"
              v-model="temp_add.type"
            />
            <label class="mb-0" for="inlineRadio1"> Chọn một </label>
          </div>
          <div class="radio radio-primary form-check-inline">
            <input
              type="radio"
              id="inlineRadio2"
              value="employee_multiple"
              name="radioInline"
              v-model="temp_add.type"
            />
            <label class="mb-0" for="inlineRadio2"> Chọn nhiều </label>
          </div>
        </div>

        <div
          class="col-lg-12 mt-2"
          v-if="
            temp_add.type == 'department' ||
            temp_add.type == 'department_multiple'
          "
        >
          <b class="col-form-label mr-3">Kiểu chọn giá trị:</b>

          <div class="radio radio-primary form-check-inline">
            <input
              type="radio"
              id="inlineRadio1"
              value="department"
              name="radioInline"
              v-model="temp_add.type"
            />
            <label class="mb-0" for="inlineRadio1"> Chọn một </label>
          </div>
          <div class="radio radio-primary form-check-inline">
            <input
              type="radio"
              id="inlineRadio2"
              value="department_multiple"
              name="radioInline"
              v-model="temp_add.type"
            />
            <label class="mb-0" for="inlineRadio2"> Chọn nhiều </label>
          </div>
        </div>

        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'file' || temp_add.type == 'file_multiple'"
        >
          <b class="col-form-label mr-3">Kiểu chọn giá trị:</b>

          <div class="radio radio-primary form-check-inline">
            <input
              type="radio"
              id="inlineRadio1"
              value="file"
              name="radioInline"
              v-model="temp_add.type"
            />
            <label class="mb-0" for="inlineRadio1"> Chọn một </label>
          </div>
          <div class="radio radio-primary form-check-inline">
            <input
              type="radio"
              id="inlineRadio2"
              value="file_multiple"
              name="radioInline"
              v-model="temp_add.type"
            />
            <label class="mb-0" for="inlineRadio2"> Chọn nhiều </label>
          </div>
        </div>
        <div class="col-lg-12 mt-2" v-if="temp_add.type == 'formular'">
          <b class="col-form-label mr-3">Lấy thông tin từ:</b>
          <div class="radio radio-primary form-check-inline">
            <input
              type="radio"
              id="inlineRadio1"
              value="1"
              name="radioInline"
              v-model="temp_add.data_setting.formular.type"
            />
            <label class="mb-0" for="inlineRadio1"> Trường dữ liệu </label>
          </div>
          <div class="radio radio-primary form-check-inline">
            <input
              type="radio"
              id="inlineRadio2"
              value="2"
              name="radioInline"
              v-model="temp_add.data_setting.formular.type"
            />
            <label class="mb-0" for="inlineRadio2"> Bảng </label>
          </div>
          <div class="mt-2" v-if="temp_add.data_setting.formular.type == 1">
            <DxHtmlEditor
              @value-changed="change_formular($event, temp_add.data_setting)"
              :value="temp_add.data_setting.formular.temp2"
              :mentions="mentions(model.fields)"
              style="min-height: 100px"
            >
              <DxValidator>
                <DxCustomRule
                  message="Công thức không hợp lệ, vui lòng kiểm tra lại!"
                  :validation-callback="validateFormular"
                />
              </DxValidator>
            </DxHtmlEditor>
            <div class="description-formular">
              <div>
                Gõ <b class="text-secondary">#</b> để chọn trường thông tin.
              </div>
              <div>
                Các phép toán có thể thực hiện: Cộng (+), Trừ (-), Nhân (*),
                Chia (/), Đóng mở ngoặc ().
              </div>
            </div>
          </div>
          <div class="row mt-2" v-if="temp_add.data_setting.formular.type == 2">
            <div class="col-6">
              <b class="col-form-label"
                >Toán tử<span class="text-danger">*</span></b
              >
              <select
                class="form-control form-control-sm"
                v-model="temp_add.data_setting.formular.operator_type"
              >
                <option value="sum">Tính tổng</option>
                <option value="avg">Tính trung bình</option>
                <option value="min">Tính Min</option>
                <option value="max">Tính Max</option>
              </select>
            </div>
            <div class="col-6">
              <b class="col-form-label"
                >Cột dữ liệu:<span class="text-danger">*</span></b
              >
              <select
                class="form-control form-control-sm"
                v-model="temp_add.data_setting.formular.operator_column"
              >
                <option
                  v-for="(column, index) in table_column(model.fields)"
                  :value="column.id"
                >
                  {{ column.text }}
                </option>
              </select>
            </div>
          </div>
          <div class="row">
            <div class="col-6">
              <b class="col-form-label"
                >Kiểu dữ liệu trả về:<span class="text-danger">*</span></b
              >
              <select
                class="form-control form-control-sm"
                v-model="temp_add.data_setting.formular.type_return"
              >
                <option value="decimal">Số thập phân</option>
                <option value="percent">Số phần trăm</option>
                <option value="currency">Tiền tệ</option>
              </select>
            </div>
            <div class="col-6">
              <b class="col-form-label"
                >Số chữ số phần thập phân:<span class="text-danger">*</span></b
              >
              <input
                class="form-control form-control-sm"
                type="number"
                v-model="temp_add.data_setting.formular.decimal_number"
              />
            </div>
          </div>
        </div>
        <div class="col-lg-12 mt-2" v-if="temp_add.type == 'table'">
          <div class="col-form-label mr-3">Thiết lập cột:</div>
          <div class="list-group-item">
            <div class="flex-m mb-2">
              <div style="margin-left: 25px; width: 300px">Tên cột</div>
              <div style="margin-left: 25px; width: 300px">Kiểu dữ liệu</div>
              <div style="margin-left: 25px">Bắt buộc</div>
            </div>
            <draggable class="" v-model="temp_add.data_setting.columns">
              <transition-group type="transition" name="flip-list">
                <div
                  class="flex-m mb-2"
                  v-for="(column, index) in temp_add.data_setting.columns"
                  :key="column.id"
                >
                  <div class="handle icon-move mr-2" style="cursor: move">
                    <i class="fas fa-grip-vertical"></i>
                  </div>

                  <input
                    class="form-control form-control-sm mr-2"
                    v-model="column.name"
                  />
                  <div class="mr-5 flex-m" style="width: 700px">
                    <select
                      class="form-control form-control-sm mr-1"
                      v-model="column.type"
                      @change="change_column_type(column)"
                    >
                      <option value="stt">STT tăng dần</option>
                      <option value="text">Một dòng</option>
                      <option value="textarea">Nhiều dòng</option>
                      <option value="number">Số</option>
                      <option value="currency">Tiền tệ</option>
                      <option value="email">Email</option>
                      <option value="yesno">Yes/no</option>
                      <option value="formular">Công thức</option>
                    </select>
                    <select
                      class="form-control form-control-sm"
                      v-if="column.type == 'currency'"
                      v-model="column.currency"
                      required
                    >
                      <option value="VND" selected>VND</option>
                      <option value="USD">DOLLAR</option>
                      <option value="EUR">EURO</option>
                    </select>
                    <div>
                      <a
                        class="btn btn-sm btn-setting"
                        @click="toggle($event, column)"
                      >
                        <i class="fas fa-cog"></i>
                      </a>
                    </div>
                  </div>
                  <div class="custom-control custom-switch switch-primary mr-2">
                    <input
                      type="checkbox"
                      class="custom-control-input"
                      :id="column.id"
                      v-model="column.is_require"
                    />
                    <label
                      class="custom-control-label"
                      :for="column.id"
                    ></label>
                  </div>
                  <div
                    class="text-danger"
                    style="cursor: pointer"
                    v-on:click="remove_column(index)"
                    v-if="temp_add.data_setting.columns.length > 1"
                  >
                    <i class="fas fa-trash-alt"></i>
                  </div>
                </div>
              </transition-group>
            </draggable>

            <div class="btn-group" role="group">
              <button
                class="btn btn-secondary btn-sm"
                v-on:click="add_column($event)"
              >
                <i class="fas fa-plus mr-2"></i> Thêm cột
              </button>
            </div>
          </div>
        </div>
        <div class="col-lg-12 mt-2">
          <b class="col-form-label">Hướng dẫn nhập:</b>
          <div class="pt-1">
            <textarea
              class="form-control form-control-sm"
              type="text"
              name="guide"
              v-model="temp_add.guide"
              placeholder="Nhập nội dung hướng dẫn"
            ></textarea>
          </div>
        </div>
        <template
          v-if="
            temp_add.type != 'table' &&
            temp_add.type != 'task' &&
            temp_add.type != 'yesno' &&
            temp_add.type != 'formular'
          "
        >
          <div class="col-lg-12 mt-2">
            <div class="checkbox checkbox-primary">
              <input
                id="checkbox2"
                type="checkbox"
                v-model="temp_add.is_require"
              />
              <label for="checkbox2"> Trường bắt buộc </label>
            </div>
          </div>
          <div
            class="col-lg-12 mt-2"
            v-if="
              temp_add.type != 'date' &&
              temp_add.type != 'date_time' &&
              temp_add.type != 'date_month' &&
              temp_add.type != 'file' &&
              temp_add.type != 'file_multiple'
            "
          >
            <div class="checkbox checkbox-primary">
              <input
                id="checkbox3"
                type="checkbox"
                v-model="temp_add.has_default"
              />
              <label for="checkbox3"> Giá trị mặc định </label>
            </div>
          </div>
        </template>
        <div class="col-lg-12 mt-2" v-if="temp_add.type == 'task'">
          <div class="checkbox checkbox-primary">
            <input
              id="checkbox3"
              type="checkbox"
              v-model="temp_add.is_require"
            />
            <label for="checkbox3">
              Bắc buộc hoàn thành toàn bộ công việc
            </label>
          </div>
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'text' && temp_add.has_default"
        >
          <input
            class="form-control form-control-sm"
            type="text"
            v-model="temp_add.data_setting.default_value"
            placeholder="Mặc định"
          />
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'number' && temp_add.has_default"
        >
          <input
            class="form-control form-control-sm"
            type="number"
            v-model="temp_add.data_setting.default_value"
            placeholder="Mặc định"
          />
        </div>

        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'currency' && temp_add.has_default"
        >
          <input
            class="form-control form-control-sm"
            type="number"
            v-model="temp_add.data_setting.default_value"
            placeholder="Mặc định"
          />
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'email' && temp_add.has_default"
        >
          <input
            class="form-control form-control-sm"
            type="email"
            v-model="temp_add.data_setting.default_value"
            placeholder="Mặc định"
          />
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'textarea' && temp_add.has_default"
        >
          <textarea
            class="form-control form-control-sm"
            type="text"
            v-model="temp_add.data_setting.default_value"
            placeholder="Mặc định"
          ></textarea>
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'select' && temp_add.has_default"
        >
          <TreeSelect
            v-model="temp_add.data_setting.default_value"
            :options="get_options(temp_add.data_setting.options)"
          >
          </TreeSelect>
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'select_multiple' && temp_add.has_default"
        >
          <TreeSelect
            v-model="temp_add.data_setting.default_value_array"
            :options="get_options(temp_add.data_setting.options)"
            multiple
          >
          </TreeSelect>
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'employee' && temp_add.has_default"
        >
          <UserTreeSelect
            v-model="temp_add.data_setting.default_value"
          ></UserTreeSelect>
        </div>

        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'employee_multiple' && temp_add.has_default"
        >
          <UserTreeSelect
            v-model="temp_add.data_setting.default_value_array"
            multiple
          ></UserTreeSelect>
        </div>

        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'department' && temp_add.has_default"
        >
          <DepartmentTreeSelect
            v-model="temp_add.data_setting.default_value"
          ></DepartmentTreeSelect>
        </div>
        <div
          class="col-lg-12 mt-2"
          v-if="temp_add.type == 'department_multiple' && temp_add.has_default"
        >
          <DepartmentTreeSelect
            v-model="temp_add.data_setting.default_value_array"
            multiple
          ></DepartmentTreeSelect>
        </div>
      </div>
      <template #footer>
        <button
          type="button"
          class="btn btn-primary"
          @click="save_field($event)"
        >
          Lưu lại
        </button>
        <button
          type="button"
          class="btn btn-secondary"
          @click="visible = false"
        >
          Hủy bỏ
        </button>
      </template>
    </Dialog>
    <OverlayPanel ref="op" :dismissable="false" showCloseIcon>
      <div class="p-3">
        <b class="col-form-label"
          >Tên biến:<span class="text-danger">*</span></b
        >
        <div class="pt-1">
          <input
            class="form-control form-control-sm"
            type="text"
            v-model="column_temp.variable"
          />
        </div>
        <div v-if="column_temp.type == 'formular'">
          <b class="col-form-label"
            >Thiết lập công thức:<span class="text-danger">*</span></b
          >
          <DxHtmlEditor
            @value-changed="change_formular($event, column_temp)"
            :value="column_temp.formular.temp2"
            :mentions="mentions_table(temp_add.data_setting.columns)"
            style="min-height: 100px"
          >
            <DxValidator>
              <DxCustomRule
                message="Công thức không hợp lệ, vui lòng kiểm tra lại!"
                :validation-callback="validateFormular"
              />
              <DxMention></DxMention>
            </DxValidator>
          </DxHtmlEditor>
          <div class="description-formular">
            <div>
              Gõ <b class="text-secondary">#</b> để chọn trường thông tin.
            </div>
            <div>
              Các phép toán có thể thực hiện: Cộng (+), Trừ (-), Nhân (*), Chia
              (/), Đóng mở ngoặc ().
            </div>
          </div>
          <div class="row">
            <div class="col-6">
              <b class="col-form-label"
                >Kiểu dữ liệu trả về:<span class="text-danger">*</span></b
              >
              <select
                class="form-control form-control-sm"
                v-model="column_temp.formular.type_return"
              >
                <option value="decimal">Số thập phân</option>
                <option value="percent">Số phần trăm</option>
                <option value="currency">Tiền tệ</option>
              </select>
            </div>
            <div class="col-6">
              <b class="col-form-label"
                >Số chữ số phần thập phân:<span class="text-danger">*</span></b
              >
              <input
                class="form-control form-control-sm"
                type="number"
                v-model="column_temp.formular.decimal_number"
              />
            </div>
          </div>
        </div>
      </div>
    </OverlayPanel>
  </div>
</template>
<script>
import stringMath from "string-math";

import {
  DxHtmlEditor,
  DxToolbar,
  DxItem,
  DxVariables,
  DxMention,
} from "devextreme-vue/html-editor";
import OverlayPanel from "primevue/overlaypanel";
import Dialog from "primevue/dialog";
import { DxValidator, DxCustomRule } from "devextreme-vue/validator";
import { rand } from "../../../../utilities/rand";
import { VueDraggableNext } from "vue-draggable-next";
// import { useProcess } from "../../../../stores/process";
import UserTreeSelect from "../../../TreeSelect/UserTreeSelect.vue";
import DepartmentTreeSelect from "../../../TreeSelect/DepartmentTreeSelect.vue";
// const store = useProcess();
export default {
  components: {
    DxHtmlEditor,
    DxToolbar,
    DxItem,
    DxVariables,
    DxValidator,
    DxCustomRule,
    draggable: VueDraggableNext,
    OverlayPanel,
    Dialog,
    DxMention,
    UserTreeSelect,
    DepartmentTreeSelect,
  },
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      myArray: [
        {
          name: "Một dòng",
          icon: '<i class="fas fa-window-minimize"></i>',
          type: "text",
        },
        {
          name: "Nhiều dòng",
          icon: '<i class="fas fa-grip-lines"></i>',
          type: "textarea",
        },
        {
          name: "Chọn giá trị",
          icon: '<i class="far fa-check-square"></i>',
          type: "select",
        },
        {
          name: "Yes/No",
          icon: '<i class="fas fa-check-circle"></i>',
          type: "yesno",
        },
        {
          name: "Thời gian",
          icon: '<i class="fas fa-clock"></i>',
          type: "date",
        },
        {
          name: "Số",
          icon: '<i class="fas fa-star"></i>',
          type: "number",
        },
        {
          name: "Tiền tệ",
          icon: '<i class="fas fa-dollar-sign"></i>',
          type: "currency",
        },

        {
          name: "Công thức",
          icon: '<i class="fas fa-calculator"></i>',
          type: "formular",
        },

        {
          name: "Bảng",
          icon: '<i class="fas fa-table"></i>',
          type: "table",
        },
        {
          name: "Email",
          icon: '<i class="fas fa-envelope"></i>',
          type: "email",
        },
        {
          name: "Tải tệp",
          icon: '<i class="fas fa-upload"></i>',
          type: "file",
        },
        {
          name: "Nhân viên",
          icon: '<i class="far fa-user-circle"></i>',
          type: "employee",
        },
        {
          name: "Bộ phận",
          icon: '<i class="fas fa-network-wired"></i>',
          type: "department",
        },
        {
          name: "Công việc con",
          icon: '<i class="fas fa-tasks"></i>',
          type: "task",
        },
      ],
      temps: [],
      active_box: false,
      temp_add: {},
      drag: false,
      formular: "",
      column_temp: {},
      visible: false,
    };
  },
  mounted() {},
  methods: {
    toggle(e, column) {
      this.column_temp = column;
      this.$refs["op"].toggle(e);
    },
    validateFormular(e) {
      var value = e.value;
      var $content = $("<div>" + value + "</div>");
      var mention = $(".dx-mention", $content);
      mention.replaceWith(function () {
        return 2;
      });
      var text = $content.text();
      var result = stringMath(text, function (err) {
        return null;
      });
      if (result) return true;
      else return false;
    },
    change_formular(e, column) {
      var value = e.value;
      column.formular.temp = value;
      console.log(e, column);
    },
    table_column(fields) {
      // console.log(fields);
      var data = [];
      var fields = fields.filter(function (item) {
        return item.type == "table";
      });

      console.log(fields);
      for (var field of fields) {
        var columns = field.data_setting.columns;
        columns = columns.filter(function (item) {
          return (
            item.type == "currency" ||
            item.type == "number" ||
            item.type == "formular"
          );
        });
        console.log(columns);
        for (var column of columns) {
          data.push({
            text: column.name,
            id: column.id,
          });
        }
      }
      return data;
    },
    mentions(fields) {
      var data = [];
      for (var field of fields) {
        if (field.type != "currency" && field.type != "number") {
          continue;
        }
        data.push({
          text: field.name,
          id: field.id,
        });
      }
      console.log(data);
      return [
        {
          dataSource: data,
          searchExpr: "text",
          displayExpr: "text",
          valueExpr: "id",
          marker: "#",
        },
      ];
    },
    mentions_table(columns) {
      var data = [];
      for (var column of columns) {
        if (column.type != "currency" && column.type != "number") {
          continue;
        }
        data.push({
          text: column.name,
          id: column.id,
        });
      }
      console.log(data);
      return [
        {
          dataSource: data,
          searchExpr: "text",
          displayExpr: "text",
          valueExpr: "id",
          marker: "#",
        },
      ];
    },
    edit(item) {
      if (item.data_setting.columns) {
        var columns = item.data_setting.columns;
        var mention = this.mentions_table(item.data_setting.columns);
        for (var column of columns) {
          if (column.type == "formular") {
            column.formular.temp2 = column.formular.text;
            for (var d of mention[0].dataSource) {
              var id = d.id;
              var html =
                `<span class="dx-mention" spellcheck="false" data-marker="#" data-mention-value="` +
                d.text +
                `" data-id="` +
                d.id +
                `">﻿<span contenteditable="false"><span>#</span>` +
                d.text +
                `</span>﻿</span>`;
              column.formular.temp2 = column.formular.temp2.replace(
                new RegExp("!#" + id + "#", "g"),
                html
              );
            }
          }
        }
      }
      if (item.type == "formular") {
        var mention = this.mentions(this.model.fields);
        item.data_setting.formular.temp2 = item.data_setting.formular.text;
        for (var d of mention[0].dataSource) {
          var id = d.id;
          var html =
            `<span class="dx-mention" spellcheck="false" data-marker="#" data-mention-value="` +
            d.text +
            `" data-id="` +
            d.id +
            `">﻿<span contenteditable="false"><span>#</span>` +
            d.text +
            `</span>﻿</span>`;
          item.data_setting.formular.temp2 =
            item.data_setting.formular.temp2.replace(
              new RegExp("!#" + id + "#", "g"),
              html
            );
        }
      }
      this.temp_add = $.extendext(true, "replace", {}, item);
      // $("#myModal").modal("show");
      this.visible = true;
    },
    copy(item) {
      this.temp_add = $.extendext(true, "replace", {}, item, { id: rand() });
      // $("#myModal").modal("show");

      this.visible = true;
    },
    add_field(type, description) {
      var default1 = {
        data_setting: {},
      };
      var temp = {
        id: rand(),
        description: description,
        name: "",
        guide: "",
        is_require: false,
        has_default: false,
        type: type,
        process_block_id: this.model.id,
      };
      temp.variable = temp.id;
      if (temp.type == "select" || temp.type == "task") {
        default1.data_setting.options = [
          {
            id: rand(),
            name: "",
          },
          {
            id: rand(),
            name: "",
          },
        ];
      }
      if (temp.type == "currency") {
        default1.data_setting.currency = "VND";
      }
      if (temp.type == "table") {
        default1.data_setting.columns = [
          {
            id: rand(),
            name: "STT",
            type: "stt",
            is_require: true,
          },
          {
            id: rand(),
            name: "Tên",
            type: "text",
            is_require: false,
          },
          {
            id: rand(),
            name: "Mô tả",
            type: "textarea",
            is_require: false,
          },
        ];
      }
      if (temp.type == "formular") {
        default1.data_setting.formular = {
          type: 1,
          text: "",
          type_return: "decimal",
          decimal_number: 0,
          operator_type: "sum",
          operator_column: "",
        };
      }
      var new_item = $.extendext(true, "replace", default1, temp);
      this.temp_add = new_item;
      // $("#myModal").modal("show");
      this.visible = true;
    },
    remove_field(id) {
      var index = this.model.fields.findIndex(function (temp) {
        return temp.id == id;
      });
      if (index != -1) {
        this.model.fields.splice(index, 1);
      }
    },
    dragOptions() {
      return {
        animation: 0,
        group: "select",
        disabled: false,
        ghostClass: "ghost",
        tag: "div",
      };
    },
    dragColumns() {
      return {
        animation: 0,
        group: "table",
        disabled: false,
        ghostClass: "ghost",
        tag: "div",
      };
    },
    dragFields() {
      return {
        animation: 0,
        group: "block_fields",
        disabled: false,
        handle: ".handle",
        ghostClass: "ghost",
        tag: "div",
      };
    },
    get_options(options) {
      return options.map(function (item) {
        item.label = item.name;
        return item;
      });
    },
    add_option(evt) {
      evt.preventDefault();
      this.temp_add.data_setting.options.push({ id: rand(), name: "" });
    },
    remove_option(index) {
      this.temp_add.data_setting.options.splice(index, 1);
    },
    add_column(evt) {
      evt.preventDefault();
      var random = rand();
      this.temp_add.data_setting.columns.push({
        id: random,
        name: "",
        variable: random,
      });
    },
    change_column_type(column) {
      if (column.type == "formular") {
        column.formular = { type: 1, text: "" };
        column.formular_text = "";
      }
    },
    remove_column(index) {
      this.temp_add.data_setting.columns.splice(index, 1);
    },
    save_field(evt) {
      // $("#myModal").modal("hide");

      this.visible = false;
      if (this.temp_add.data_setting.columns) {
        for (var column of this.temp_add.data_setting.columns) {
          if (column.type == "formular" && column.formular.temp) {
            var firstvariable = "!#"; //first input;
            var secondvariable = "#"; //first in
            var $content = $("<div>" + column.formular.temp + "</div>");
            var mention = $(".dx-mention", $content);
            mention.replaceWith(function () {
              var id = $(this).data("id");
              return firstvariable + id + secondvariable;
            });
            column.formular.text = $content.text();
          }
        }
      }
      if (
        this.temp_add.type == "formular" &&
        this.temp_add.data_setting.formular.temp
      ) {
        var firstvariable = "!#"; //first input;
        var secondvariable = "#"; //first in
        var $content = $(
          "<div>" + this.temp_add.data_setting.formular.temp + "</div>"
        );
        var mention = $(".dx-mention", $content);
        mention.replaceWith(function () {
          var id = $(this).data("id");
          return firstvariable + id + secondvariable;
        });
        this.temp_add.data_setting.formular.text = $content.text();
      }
      var item = $.extendext(true, "replace", {}, this.temp_add);
      if (!this.model.fields) {
        this.model.fields = [];
      }
      var id = item.id;
      var index = this.model.fields.findIndex(function (temp) {
        return temp.id == id;
      });
      if (index != -1) {
        this.model.fields[index] = item;
      } else {
        this.model.fields.push(item);
      }
      this.temp_add = {};
    },
  },
};
</script>
<style scoped>
.btn-setting {
  font-size: 18px;
  padding: 6px 10px;
  margin-left: 5px;
  box-shadow: none;
  border: 1px solid #dfdfdf;
}

.dropdown-menu-center {
  right: auto;
  left: 50%;
  -webkit-transform: translate(-50%, 0);
  -o-transform: translate(-50%, 0);
  transform: translate(-50%, 0);
}
</style>
