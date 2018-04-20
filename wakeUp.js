const axios = require('axios')

var instance = axios.create({
  baseURL: 'https://cp.remotethings.co.uk/api',
  timeout: 5000,
  headers: {'Authorization': 'pT4kQ4VifoHxQp6m27cbAbitEhs4Cs0zlE1KtV9Z9iv2r2871uT0QVarG1D991DC'},
  params: {
    duration: 1
  }
})

instance.get('/devices/463/wakeUp').then((resp) => {
  console.log('resp: ', resp)
})
