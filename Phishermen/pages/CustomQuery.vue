<template>
  <div>
    <!--This is stuff most users wouldn't care about and the page is already too bloated so all this hidden unless a button is clicked -->
    <div v-if="advancedDisplay">
      <v-text-field v-model="queryName" label="Enter a Name for Your Query" style="width='700px'" />
      <v-btn @click="saveQuery()">
        Save Query
      </v-btn>
      <v-autocomplete
        v-model="selectedUserQuery"
        :items="userQueries"
        item-text="queryName"
        item-value="queryUrl"
        label="Select a user query."
        @change="fillTable(selectedUserQuery)"
      />
      <span>Selected User Query URL: {{ selectedUserQuery }}</span>
      <br>
      <p>Current Query URL: {{ completeURL }}</p>
      <v-btn @click="fillTable()">
        Refresh Table
      </v-btn>
      <br>
    </div> <!--End of v-if"advancedDisplay" -->
    <v-btn @click="advancedDisplay = !advancedDisplay">
      Advanced Display
    </v-btn>
    <v-btn @click="resetURL()">
      Reset
    </v-btn>
    <v-row>
      <v-col>
        <!-- The list of current filters -->
        <v-list>
          <v-subheader>Filters(Click to Remove):</v-subheader>
          <v-list-item-group v-model="selectedFilter">
            <v-list-item v-for="(item, i) in filters" :key="i" :value="item.Value" @click="removeFilter(item.Value)">
              <v-list-item-content>
                <v-list-item-title v-text="item.Text" />
              </v-list-item-content>
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-col>
      <!-- Start of User Inputs -->
      <v-col>
        <v-autocomplete v-model="selectedYear" :items="validYears" label="Select a Year" />

        <v-btn @click="addFilter('years', selectedYear)">
          Add Year Filter
        </v-btn>
      </v-col>
      <v-col>
        <v-autocomplete v-model="selectedMonth" :items="validMonths" item-text="monthName" item-value="monthNumber" label="Select a Month" />
        <v-btn @click="addFilter('months', selectedMonth)">
          Add Month Filter
        </v-btn>
      </v-col>
      <v-col>
        <v-autocomplete v-model="selectedArea" :items="validAreas" :item-text="item => item.areaNumber +': '+ item.areaName" item-value="areaNumber" label="Select an Area" />
        <v-btn @click="addFilter('areaNumbers', selectedArea)">
          Add Area Filter
        </v-btn>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <v-autocomplete v-model="selectedRegion" :items="validRegions" label="Select a Region" />
        <v-btn @click="addFilter('regions', selectedRegion)">
          Add Region Filter
        </v-btn>
      </v-col>

      <v-col>
        <v-autocomplete v-model="selectedSystem" :items="validSystems" label="Select a System" />
        <v-btn @click="addFilter('systems', selectedSystem)">
          Add System Filter
        </v-btn>
      </v-col>

      <v-col>
        <v-autocomplete v-model="selectedGroupBy" :items="validGroupBys" label="Select a Column To Group" @change="fillTable()" />
      </v-col>

      <v-col>
        <v-autocomplete v-model="selectedAggregate" :items="validAggregates" label="Select Group Data" @change="fillTable()" />
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
        <v-select v-model="selectedSort" :items="validSorts" label="Select Sort Order" @change="fillTable()" />
      </v-col>

      <v-col>
        <!-- We need to display different sorts depending on whether or not the current query is using a group by -->
        <div v-if="groupedDisplay">
          <v-select v-model="selectedGroupSortBy" :items="validGroupSortBy" label="Select Column To Sort By" @change="fillTable()" />
        </div>
        <div v-else>
          <v-autocomplete v-model="selectedSortBy" :items="validSortBy" label="Select Column To Sort By" @change="fillTable()" />
        </div>
      </v-col>
      <!--End of user inputs -->
    </v-row>
    <br>
    <!-- Here is where we display our query results -->
    <!-- This is to display different column headers based on whether a query is using a group by -->
    <div v-if="!groupedDisplay">
      <v-data-table :items="customQueryResults" :headers="headers" />
    </div>
    <div v-else>
      <v-data-table :items="customQueryResults" :headers="groupHeaders" />
    </div>
  </div>
</template>

<script>
export default {

  data () {
    return {
      // holds the data we get back from axios calls to the API
      customQueryResults: [],
      // column headers for dataTables
      headers: [
        { text: 'Year', value: 'year' },
        { text: 'Month', value: 'month' },
        { text: 'Area Number', value: 'areaNumber' },
        { text: 'Area Name', value: 'areaName' },
        { text: 'Fish Caught', value: 'fishCaught' },
        { text: 'System', value: 'system' },
        { text: 'Region', value: 'region' }
      ],

      groupHeaders: [
        { text: 'Group Name', value: 'groupKey' },
        { text: 'Fish Caught', value: 'fishCaught' }
      ],
      // baseURL contains the default URL that we use at the start
      baseURL: '../api/CustomQuery?',
      // url is the "current" url with filters and what not added
      url: '../api/CustomQuery?',
      // used when converting a month number to a month name
      months: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
      // these are used to populate inputs by calling the api to get the data using the given URL and
      // then saving the data in the appropriate array
      validYears: [],
      validYearsURL: '../api/GetYears',
      validMonths: [],
      validMonthsURL: '../api/GetMonths',
      validAreas: [],
      validAreasURL: '../api/GetAreas',
      validRegions: [],
      validRegionsURL: '../api/GetRegions',
      validSystems: [],
      validSystemsURL: '../api/GetSystems',
      // hard coded values used to populate inputs
      validGroupBys: ['None', 'Date', 'Year', 'Month', 'AreaNumber', 'System', 'Region'],
      validAggregates: ['Sum', 'Average', 'Max', 'Min'],
      validSorts: ['Ascending', 'Descending'],
      validSortBy: ['Year', 'Month', 'System', 'AreaNumber', 'Region', 'FishCaught'],
      validGroupSortBy: ['Group Name', 'Fish Caught'],
      // used when retrieving and displaying user made queries from the database
      saveQueryURL: '../api/SaveQuery?queryURL=',
      getUserQueriesURL: '../api/GetUserQueries',
      userQueries: [],
      // holds the filters applied to the current query
      filters: [],
      // toggles the display or certain portions of the page
      advancedDisplay: false,
      groupedDisplay: false,
      queryName: '',
      // holds the values selected by the user using the page inputs
      selectedRegion: null,
      selectedArea: null,
      selectedMonth: 'Select A Month',
      selectedYear: null,
      selectedSystem: null,
      selectedAggregate: 'Average',
      selectedGroupBy: 'None',
      selectedUserQuery: null,
      selectedSort: 'Ascending',
      selectedSortBy: 'AreaNumber',
      selectedGroupSortBy: 'Group Name',
      selectedFilter: null,
      haulGreaterThan: 0,
      haulLessThan: 10000,
      rows: 1000
    }
  },
  // this calculates the full URL to be used to call the controller
  computed: {
    completeURL () {
      // v-btns to add filters that can have multiple values are fine but some
      // values should only appear once so this code adds all the values you can only have once to the url
      let url = this.url + 'haulGreaterThan=' + this.haulGreaterThan + '&'
      url += 'haulLessThan=' + this.haulLessThan + '&'
      url += 'rows=' + this.rows + '&'
      url += 'groupBy=' + this.selectedGroupBy + '&'
      if (this.selectedSort === 'Descending') {
        url += 'sortAscending=false&'
      }
      if (this.groupedDisplay) {
        url += 'sortBy=' + this.selectedGroupSortBy + '&'
        url += 'aggregate=' + this.selectedAggregate + '&'
      } else {
        url += 'sortBy=' + this.selectedSortBy + '&'
      }
      return url
    }

  },
  // populates all our inputs on page load
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
    // gets the data from the current query and displays it in the appropriate data table.
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
    // populates inputs with proper values from the database
    // I hate having all these different functions but I don't know how to do something like a C# out parameter in javascript.
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
    // adds a chosen filter to our list of filters and the actual query url
    addFilter (filterType, value) {
      const filterText = filterType + '=' + value + '&'

      // this little bit is so we display the month name in the list of filters instead of the month number
      if (filterType === 'months') {
        value = this.months[value - 1]
      }
      // and this bit is to display an area name and number instead of just the number which is confusing
      if (filterType === 'areaNumbers') {
        const rightAreaNumber = element => element.areaNumber === value
        const index = this.validAreas.findIndex(rightAreaNumber)
        value = this.validAreas[index].areaNumber + ': ' + this.validAreas[index].areaName
      }
      // this looks kind of dumb but it's intentional
      // in the listbox we want to show the value that's being filtered
      // but we need the full string in the background to remove it from the url
      const filter = { Text: value, Value: filterText }

      this.filters.push(filter)
      this.url += filterText
      this.fillTable()
    },
    // removes a filter from the url/filter list and displays the new results
    removeFilter (filter) {
      this.filters = this.filters.filter(e => e.Value !== filter)
      this.url = this.url.replace(filter, '')
      this.fillTable()
    },
    // this saves the current query with the user given name to the database
    saveQuery () {
      const url = this.completeURL
      const queryName = this.queryName
      const saveQueryURL = this.saveQueryURL
      const finalURL = saveQueryURL + encodeURIComponent(url) + '&queryName=' + queryName
      alert(finalURL)
      this.$axios.post(finalURL).then((value) => { alert('Query Saved') })
    },
    // resets the URL back to the default URL and removes all filters
    resetURL () {
      this.url = this.baseURL
      this.selectedSortBy = 'AreaNumber'
      this.selectedGroupBy = 'None'
      this.filters = []
      this.fillTable()
    }
  }
}
</script>

<style>
.v-select {
    width:250px;
    color:"red";

}

.v-list {
  height: 200px;
  overflow-y: auto;
}
</style>
