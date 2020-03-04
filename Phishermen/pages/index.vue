<style>
  .modal {
    border-radius: 5px;
    color: #333;
    font-family: sans-serif;
    line-height: 1.5;
    margin: 15% auto auto;
    width:auto;
    max-width: fit-content;
    padding: 1rem 2rem;
    backdrop-filter: blur(1px);
    background-color: rgba(0, 0, 0, 0.5);
  a {
    color: #bf0222;
  }
  }
  .content-container {
    align-items: center;
    justify-content: center;
    position: center;
    padding-left: 5px;
    padding-right: 5px;
    margin-bottom: -40px;
    height: 100%;
    width: auto;
  }
</style>

<template>
  <v-container>
    <v-row>
      <h1 class="display-3 green--text font-weight-bold" style="margin: auto">
        WELCOME FISHERMEN
      </h1>
    </v-row>
    <br>
    <!--Displaying the carousel-->
    <v-carousel
      cycle
      interval="5000"
      show-arrows-on-hover
      hide-delimiters
    >
      <v-carousel-item
        v-for="(item, i) in items"
        :key="i"
        reverse-transition="fade-transition"
        :src="item.src"
        style="align-content: center"
      >
        <h3 style="position: absolute; bottom: 0; right: 0; text-align: right; width: 100%;" class="display-3 lime--text mb-10 font-weight-bold">
          <div class="content-container">
            <div class="modal">
              <div style="text-shadow: 0 3px black;position: relative;z-index: 1;background-size: 30px 30px ; color:white; position: relative;top: 30%; width: 100%; padding-left: 10px; padding-right:10px; text-align: center;">
                <h5 v-if="locationss[i] !== ''">
                  <a :href="locationss[i]">
                    {{ messages[i] }}
                  </a>
                </h5>
                <h5 v-else :href="locationss[i]">
                  {{ messages[i] }}
                </h5>
              </div>
            </div>
          </div>
        </h3>
      </v-carousel-item>
    </v-carousel>
    <br>
    <!--middle infor -->
    <v-row>
      <v-col
        cols="6"
      >
        <v-card
          class="pa-1"
          flat
        >
          <div class="align-self-center text-center">
            <p class="font-weight-light font-italic" style="font-size: 2em">
              <br>
              "Most of the Puget Sound fishery for Dungeness crab takes place from Everett northward,
              with the bulk of the harvest in the Blaine/Point Roberts area."
            </p>
          </div>
        </v-card>
      </v-col>
      <!--colomn 2 -->
      <v-col
        cols="6"
      >
        <v-card
          class="pa-1"
          flat
        >
          <v-img
            class="white--text align-end"
            height="400px"
            flat
            src="/images/puget.jpg"
          >
            <v-card-title>Puget Sound</v-card-title>
          </v-img>
        </v-card>
      </v-col>
    </v-row>
    <!--displying cards-->
    <br>
    <v-row
      md2
    >
      <span style="margin: auto;">
        <h3 class="display-2 grey--text font-weight-bold">REGIONS</h3>
      </span>
    </v-row>
    <v-layout
      row
      wrap
    >
      <v-flex
        v-for="(im, index) in regions"
        :key="im.ID"
        xs12
        sm6
        md2
        lg3
      >
        <v-card
          class="text-xs-center ma-3"
        >
          <v-img
            class="white--text align-end"
            height="200px"
            :src="im.src"
          >
            <v-card-title>{{ im.Name }}</v-card-title>
          </v-img>
          <v-card-text>
            <div class="subheading">
              Region ID: {{ im.ID }}
            </div>
            <div class="grey--text">
              Region Name: {{ im.Name }}
            </div>
          </v-card-text>
          <v-card-actions>
            <v-btn color="red" block @click="setIsDialog(index)">
              TOP TEN
            </v-btn>
            <TopTenDialog v-if="isDialog[index]" :index="index" :region-name="im.Name" @setIsDialog="setIsDialog" />
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
    <br>

    <!--wrap up info-->

    <v-row>
      <v-col
        cols="6"
      >
        <v-card
          class="pa-1"
          flat
        >
          <v-img
            class="white--text align-end"
            height="400px"
            flat
            src="/images/fish.jpg"
          >
            <v-card-title>Salmon</v-card-title>
          </v-img>
        </v-card>
      </v-col>
      <!-- column 2 -->
      <v-col
        cols="6"
      >
        <v-card
          class="pa-1"
          flat
        >
          <div class="align-self-center text-center">
            <p class="font-weight-light font-italic" style="font-size: 2em">
              <br>
              salmon are anadromous: they hatch in fresh water, migrate to the ocean,
              then return to fresh water to reproduce
            </p>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <!--footer-->
    <v-footer
      dark
      padless
    >
      <v-card
        flat
        tile
        class="green white--text text-center"
      >
        <v-card-text>
          <v-btn
            v-for="icon in icons"
            :key="icon"
            class="mx-4 white--text"
            icon
          >
            <v-icon size="24px">
              {{ icon }}
            </v-icon>
          </v-btn>
        </v-card-text>
        <v-divider />

        <v-card-text class="white--text pt-0">
          Some of the best fishing opportunities in the nation are available in Washington. From fly-fishing for bass and trout on freshwater lakes and streams east of the Cascades to trolling for salmon along the coast to crabbing in Puget Sound, Washington offers a diverse and unique outdoors experience. Find the experience that's right for you, whether you're a long-time angler or a first-time fisher.
        </v-card-text>

        <v-divider />

        <v-card-text class="white--text">
          {{ new Date().getFullYear() }} — <strong>CodeSmart Inc</strong>
        </v-card-text>
      </v-card>
    </v-footer>
  </v-container>
</template>

<script>

import TopTenDialog from '../components/TopTenDialog'
export default {
  components: { TopTenDialog },
  data: () => ({
    isDialog: [false, false, false, false],
    items: [
      {
        src: '/images/YakRiver.jpg'
      },
      {
        src: '/images/12_Fishing.jpg'
      },
      {
        src: '/images/Steelhead-Washington-state-fish.jpg'
      },
      {
        src: '/images/Fleet.jpg'
      }
    ],
    locationss: [

      '/bestplace',
      '/bestmonth',
      '',
      ''
    ],
    messages: [
      'Fish for the Best Location!',
      'Fish for the Best Season!',
      'The Fishermen website is the Best tool to find the Best Fishing!',
      'All in Washington State!'
    ],
    regions: [
      { ID: 6, Name: 'COASTAL', src: '/images/coastal.jpg' },
      { ID: 7, Name: 'COLUMBIA', src: '/images/Columbia.jpg' },
      { ID: 8, Name: 'PUGET SOUND', src: '/images/puget.jpg' },
      { ID: 9, Name: 'UNKNOWN', src: '/images/unknown.jpg' }
    ],
    icons: [
      'mdi-facebook',
      'mdi-twitter',
      'mdi-google-plus',
      'mdi-linkedin',
      'mdi-instagram'
    ]
  }),
  methods: {
    setIsDialog (index) {
      this.$set(this.isDialog, index, !this.isDialog[index])
    }
  }
}
</script>
