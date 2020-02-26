<template>
  <v-autocomplete
    v-model="model"
    :items="items"
    :loading="isLoading"
    :search-input.sync="search"
    color="white"
    hide-no-data
    hide-selected
    filled
    chips
    multiple
    item-text="areaName"
    item-value="areaNumber"
    label="Public APIs"
    placeholder="Start typing to Search"
    prepend-icon="mdi-database-search"
    return-object
  >
    <template v-slot:selection="data">
      <v-chip
        v-bind="data.attrs"
        :input-value="data.selected"
        close
        @click="data.select"
        @click:close="remove(data.item)"
      >
        {{ data.item.areaName }}
      </v-chip>
    </template>
  </v-autocomplete>
</template>

<script>
export default {
  name: 'SearchBox',
  data: () => ({
    descriptionLimit: 10,
    entries: [],
    isLoading: false,
    model: null,
    search: null
  }),
  computed: {
    items() {
      return this.entries
    }
  },

  watch: {
    search(val) {
      // Items have already been loaded
      // if (this.model !== null)
      //   console.log(this.model.areaName + this.model.areaNumber)
      if (this.items.length > 0) return

      // Items have already been requested
      if (this.isLoading) return

      this.isLoading = true

      // Lazily load input items
      this.$axios
        .$get('GetAreas')
        .then((res) => {
          console.log(res)
          this.entries = res
        })
        .catch((err) => {
          console.log(err)
        })
        .finally(() => (this.isLoading = false))
    }
  },
  methods: {
    remove(item) {
      const index = this.model.findIndex((a) => a.areaName === item.areaName)
      if (index >= 0) this.model.splice(index, 1)
    }
  }
}
</script>

<style scoped></style>
