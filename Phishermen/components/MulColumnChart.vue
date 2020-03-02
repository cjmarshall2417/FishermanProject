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
  name: 'MulColumnChart',
  props: {
    data: {
      default () {
        return ['100']
      },
      type: Array
    }
  },
  computed: {
    series () {
      return this.data
    },
    chartOptions () {
      return {
        chart: {
          type: 'bar',
          height: 350
        },
        plotOptions: {
          bar: {
            horizontal: false,
            columnWidth: '55%',
            endingShape: 'rounded'
          }
        },
        dataLabels: {
          enabled: false
        },
        stroke: {
          show: true,
          width: 2,
          colors: ['transparent']
        },
        xaxis: {
          categories: [
            'Jan',
            'Feb',
            'Mar',
            'Apr',
            'May',
            'Jun',
            'Jul',
            'Aug',
            'Sep',
            'Oct',
            'Nov',
            'Dec'
          ]
        },
        yaxis: {
          title: {
            text: 'Number of fish'
          }
        },
        fill: {
          opacity: 1
        },
        tooltip: {
          y: {
            formatter (val) {
              return val + ' fishes'
            }
          }
        }
      }
    }
  }
}
</script>

<style scoped></style>
