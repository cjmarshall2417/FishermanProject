<template>
  <div>
    <v-card color="red lighten-2" dark>
      <v-card-title class="headline red lighten-3">
        Best Place to fish in a month
      </v-card-title>
      <v-card-text>
        Search an area to find out the best place to fish in a month
      </v-card-text>
      <v-card-text>
        <SearchBox
          label="System"
          url="GetSystems"
          :is-multiple="true"
          placeholder="Search System"
          @system="getSystem"
        />
        <SearchBox
          label="Month"
          item-value="monthNumber"
          item-text="monthName"
          :is-obj="true"
          url="GetMonths"
          placeholder="Search Month"
          @month="getMonth"
        />
      </v-card-text>
      <v-card-actions>
        <v-btn @click="submit">
          Submit
        </v-btn>
      </v-card-actions>
    </v-card>
    <ColumnChart v-if="isSubmit" :data="data" :xaxis="xaxis" />
  </div>
</template>

<script>
import SearchBox from '../components/SearchBox'
import ColumnChart from '../components/ColumnChart'
export default {
  components: { ColumnChart, SearchBox },
  data: () => ({
    data: [],
    xaxis: [],
    isSubmit: false
  }),
  methods: {
    getMonth(month) {
      this.month = month
    },
    getSystem(system) {
      this.system = system
    },
    submit() {
      // console.log(this.system)
      if (this.system === undefined || this.month === undefined) return
      const formData = new FormData()
      for (let i = 0; i < this.system.length; i++) {
        formData.append('listOfSystems', this.system[i])
      }
      formData.append('month', this.month.monthNumber)

      this.data = []
      this.xaxis = []
      this.$axios
        .$post(`BestPlaceToFishDuringMonth`, formData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        })
        .then((res) => {
          for (let i = 0; i < res.length; i++) {
            this.data.push(res[i].averageHaul)
            this.xaxis.push(res[i].areaName || 'null')
          }
        })
        .then(() => {
          this.isSubmit = true
        })
    }
  }
}
</script>
