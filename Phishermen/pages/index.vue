<template>
  <v-card color="red lighten-2" dark>
    <v-card-title class="headline red lighten-3">
      Search for Public APIs
    </v-card-title>
    <v-card-text>
      Explore hundreds of free API's ready for consumption! For more information
      visit
      <a
        class="grey--text text--lighten-3"
        href="https://github.com/toddmotto/public-apis"
        target="_blank"
        >the Github repository</a
      >.
    </v-card-text>
    <v-card-text>
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
    </v-card-text>
    <v-card-actions>
      <v-btn @click="submit">
        Submit
      </v-btn>
      <v-spacer></v-spacer>
      <v-btn :disabled="!model" color="grey darken-3" @click="model = null">
        Clear
        <v-icon right>mdi-close-circle</v-icon>
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
export default {
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
      // return this.entries.map((entry) => {
      //   const Description =
      //     entry.areaName.length > this.descriptionLimit
      //       ? entry.areaName.slice(0, this.descriptionLimit) + '...'
      //       : entry.areaName
      //
      //   return Object.assign({}, entry, { Description })
      // })
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
      fetch('https://localhost:5001/api/GetAreas')
        .then((res) => res.json())
        .then((res) => {
          // console.log(res)
          // const { count, entries } = res
          // this.count = count
          this.entries = res
        })
        .catch((err) => {
          console.log(err)
        })
        .finally(() => (this.isLoading = false))
    }
  },
  methods: {
    submit() {
      console.log(this.model)
    },
    remove(item) {
      const index = this.model.findIndex((a) => a.areaName === item.areaName)
      if (index >= 0) this.model.splice(index, 1)
    }
  }
}
</script>
