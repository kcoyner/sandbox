const axios = require('axios')

let start = new Date('2018-03-21T05:20:30.000Z')
let end = new Date('2018-03-21T05:23:30.000Z')

let startE = start.getTime()
let endE = end.getTime()

var instance = axios.create({
  baseURL: 'https://cp.remotethings.co.uk/api',
  timeout: 5000,
  headers: {'Authorization': 'pT4kQ4VifoHxQp6m27cbAbitEhs4Cs0zlE1KtV9Z9iv2r2871uT0QVarG1D991DC'},
  params: {
    filter: {
      where: {
        timestamp: {
          between: [startE, endE]
        }
      }
    }
  }
})

instance.get('/devices/463/points').then((resp) => {
  console.log('resp.data: ', resp.data)
})
