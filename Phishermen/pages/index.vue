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
      <SearchBox />
    </v-card-text>
    <v-card-actions>
      <!--      <v-btn @click="submit">-->
      <!--        Submit-->
      <!--      </v-btn>-->
      <!--      <v-spacer></v-spacer>-->
      <!--      <v-btn :disabled="!model" color="grey darken-3" @click="model = null">-->
      <!--        Clear-->
      <!--        <v-icon right>mdi-close-circle</v-icon>-->
      <!--      </v-btn>-->
    </v-card-actions>
  </v-card>
</template>

<script>
import SearchBox from '../components/SearchBox'
export default {
  components: { SearchBox },
  methods: {
    submit() {
      console.log(this.model)
    }
  }
}
</script>
