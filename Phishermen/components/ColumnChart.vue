<template>
  <div id="chart">
    <apexchart
      type="bar"
      height="350"
      :options="chartOptions"
      :series="series"
    />
  </div>
</template>

<script>
export default {
  name: 'ColumnChart',
  props: {
    data: {
      default () {
        return ['1']
      },
      type: Array
    },
    xaxis: {
      default () {
        return ['a']
      },
      type: Array
    }
  },
  computed: {
    series () {
      return [
        {
          name: 'Fish',
          data: this.data
        }
      ]
    },
    chartOptions () {
      return {
        chart: {
          height: 350,
          type: 'bar',
          events: {
            click (chart, w, e) {
              // console.log(chart, w, e)
            }
          }
        },
        plotOptions: {
          bar: {
            columnWidth: '45%',
            distributed: true
          }
        },
        dataLabels: {
          enabled: false
        },
        legend: {
          show: false
        },
        xaxis: {
          categories: this.xaxis,
          labels: {
            style: {
              fontSize: '12px'
            }
          }
        }
      }
    }
  }
}
</script>

<style scoped></style>
