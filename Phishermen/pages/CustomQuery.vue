
<template>
  <div>
    <span>Query Name: </span><input v-model="queryName" style="width: 700px" type="text">
    <v-btn @click="saveQuery()">
      Save Query
    </v-btn>
    <v-select v-model="selectedUserQuery" :items="userQueries" label="Select a user query." @change="fillTable(selectedUserQuery)" />

    <p>{{ completeURL }}</p>
    <v-btn @click="fillTable()">
      Refresh Table
    </v-btn><v-btn @click="resetURL()">
      Reset
    </v-btn><br>
    <v-row>
      <v-col>
        <v-select v-model="selectedYear" :items="validYears" label="Select a Year" />

        <v-btn @click="addFilter('years', selectedYear)">
          Add Year Filter
        </v-btn>
      </v-col>
      <v-col>
        <v-select v-model="selectedMonth" :items="validMonths" item-text="monthName" item-value="monthNumber" label="Select a Month" />
        <v-btn @click="addFilter('months', selectedMonth)">
          Add Month Filter
        </v-btn>
      </v-col>
      <v-col>
        <v-select v-model="selectedArea" :items="validAreas" :item-text="item => item.areaNumber +': '+ item.areaName" item-value="areaNumber" label="Select an Area" />
        <v-btn @click="addFilter('areaNumbers', selectedArea)">
          Add Area Filter
        </v-btn>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-select v-model="selectedRegion" :items="validRegions" label="Select a Region" />
        <v-btn @click="addFilter('regions', selectedRegion)">
          Add Region Filter
        </v-btn>
      </v-col>

      <v-col>
        <v-select v-model="selectedSystem" :items="validSystems" label="Select a System" />
        <v-btn @click="addFilter('systems', selectedSystem)">
          Add System Filter
        </v-btn>
      </v-col>

      <v-col>
        <v-select v-model="selectedGroupBy" :items="validGroupBys" label="Choose a Group By" @change="fillTable()" />
      </v-col>

      <v-col>
        <v-select v-model="selectedAggregate" :items="validAggregates" label="Choose an Aggregate" @change="fillTable()" />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <span>Haul greater than:</span> <input v-model="haulGreaterThan" type="text" @blur="fillTable()">
      </v-col>

      <v-col>
        <span>Haul less than:</span> <input v-model="haulLessThan" type="text" @blur="fillTable()">
      </v-col>

      <v-col>
        <span>Rows to return:</span> <input v-model="rows" type="text" @blur="fillTable()">
      </v-col>

      <v-col>
        <v-select v-model="selectedSort" :items="validSorts" @change="fillTable()" />
      </v-col>

      <v-col>
        <v-select v-model="selectedSortBy" :items="validSortBy" @change="fillTable()" />
      </v-col>
    </v-row>
    <br>
    <div v-if="!groupedDisplay">
      <v-data-table :items="customQueryResults" :headers="headers"></v-data-table>
    <br>
    <table>
        <tr>
          <th>Year</th>
          <th>Month</th>
          <th>Area Number</th>
          <th>Area Name</th>
          <th>Fish Caught</th>
          <th>System</th>
          <th>Region</th>
        </tr>
        <tr v-for="(haul,index) in customQueryResults" :key="index">
          <td>{{ haul["year"] }}</td>
          <td>{{ haul["month"] }}</td>
          <td>{{ haul["areaNumber"] }}</td>
          <td>{{ haul["areaName"] }}</td>
          <td>{{ haul["fishCaught"] }}</td>
          <td>{{ haul["system"] }}</td>
          <td>{{ haul["region"] }}</td>
        </tr>
      </table>
    </div>
    <div v-else>
      <table>
        <tr>
          <th>Group Key</th>
          <th>Fish Caught</th>
        </tr>
        <tr v-for="(haul,index) in customQueryResults" :key="index">
          <td>{{ haul["groupKey"] }}</td>
          <td>{{ haul["fishCaught"] }}</td>
        </tr>
      </table>
    </div>
  </div>
</template>

<script>
export default {

  data () {
    return {
      customQueryResults: [],

      headers: [
        { text: 'Year', value: 'year' },
        { text: 'Month', value: 'month' },
        { text: 'Area Number', value: 'areaNumber' },
        { text: 'Area Name', value: 'areaName' },
        { text: 'Fish Caught', value: 'fishCaught' },
        { text: 'System', value: 'system' },
        { text: 'Region', value: 'region' }
      ],

      baseURL: '../api/CustomQuery?',
      url: '../api/CustomQuery?',
      months: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],

      validYears: [],
      validYearsURL: 'GetYears',
      validMonths: [],
      validMonthsURL: 'GetMonths',
      validAreas: [],
      validAreasURL: '../api/GetAreas',
      validRegions: [],
      validRegionsURL: '../api/GetRegions',
      validSystems: [],
      validSystemsURL: '../api/GetSystems',
      validGroupBys: ['None', 'Date', 'Year', 'Month', 'AreaNumber', 'System', 'Region'],
      validAggregates: ['Sum', 'Average'],
      validSorts: ['Ascending', 'Descending'],
      validSortBy: ['Group Name', 'Fish Caught'],
      saveQueryURL: '../api/SaveQuery?queryURL=',
      getUserQueriesURL: '../api/GetUserQueries',
      userQueries: [],

      groupedDisplay: false,
      queryName: '',
      selectedRegion: null,
      selectedArea: null,
      selectedMonth: 'Select A Month',
      selectedYear: null,
      selectedSystem: null,
      selectedAggregate: 'Average',
      selectedGroupBy: 'None',
      selectedUserQuery: null,
      selectedSort: 'Ascending',
      selectedSortBy: 'Group Name',
      haulGreaterThan: 0,
      haulLessThan: 10000,
      rows: 1000
    }
  },

  computed: {
    completeURL () {
      // v-btns to add filters that can have multiple values are fine but some
      // values should only appear once so this code adds all the values you can only have once to the url
      let url = this.url + 'haulGreaterThan=' + this.haulGreaterThan + '&'
      url += 'haulLessThan=' + this.haulLessThan + '&'
      url += 'rows=' + this.rows + '&'
      url += 'groupBy=' + this.selectedGroupBy + '&'
      if (this.selectedAggregate === 'Sum') {
        url += 'average=false&'
      }

      if (this.selectedSort === 'Descending') {
        url += 'sortAscending=false&'
      }

      if (this.selectedSortBy === 'Fish Caught') {
        url += 'sortByKey=false&'
      }

      return url
    }

  },
  mounted () {
    this.fillTable()
    this.getYears()
    this.getMonths()
    this.getAreas()
    this.getRegions()
    this.getSystems()
    this.getUserQueries()
  },
  methods: {
    fillTable (url = '') {
      if (url === '') {
        url = this.completeURL
      }

      if (url.includes('groupBy=None')) {
        this.groupedDisplay = false
      } else {
        this.groupedDisplay = true
      }

      this.$axios.$get(url)
        .then((value) => { this.customQueryResults = value })
    },

    // I hate having all these different functions but I don't know how to do something like a C# out keyword in javascript.
    getYears () {
      this.$axios.$get(this.validYearsURL)
        .then((value) => { this.validYears = value })
    },

    getMonths () {
      this.$axios.$get(this.validMonthsURL)
        .then((value) => { this.validMonths = value })
    },

    getAreas () {
      this.$axios.$get(this.validAreasURL)
        .then((value) => { this.validAreas = value })
    },

    getRegions () {
      this.$axios.$get(this.validRegionsURL)
        .then((value) => { this.validRegions = value })
    },

    getSystems () {
      this.$axios.$get(this.validSystemsURL)
        .then((value) => { this.validSystems = value })
    },

    getUserQueries () {
      this.$axios.$get(this.getUserQueriesURL)
        .then((value) => { this.userQueries = value })
    },

    addFilter (filterType, value) {
      this.url += filterType + '=' + value + '&'
      this.fillTable()

      // code taken from https://stackoverflow.com/questions/9539723/javascript-to-select-first-option-of-select-list
      const selectTags = document.body.getElementsByTagName('select')

      for (let i = 0; i < selectTags.length; i++) {
        selectTags[i].selectedIndex = '0'
      }
    },

    saveQuery () {
      const url = this.completeURL
      const queryName = this.queryName
      const saveQueryURL = this.saveQueryURL
      const finalURL = saveQueryURL + encodeURIComponent(url) + '&queryName=' + queryName
      alert(finalURL)
      this.$axios.post(finalURL).then((value) => { alert('Query Saved') })
    },

    resetURL () {
      this.url = this.baseURL
    },

    buildCompleteURL () {
      // v-btns to add filters that can have multiple values are fine but some
      // values should only appear once so this code adds all the values you can only have once to the url
      let url = this.url + 'haulGreaterThan=' + this.haulGreaterThan + '&'
      url += 'haulLessThan=' + this.haulLessThan + '&'
      url += 'rows=' + this.rows + '&'
      url += 'groupBy=' + this.selectedGroupBy + '&'
      if (this.selectedAggregate === 'Average') {
        url += 'average=true&'
      } else {
        url += 'average=false&'
      }

      return url
    }
  }

}
</script>

<style>
.v-select {
    width:250px;

}
</style>
