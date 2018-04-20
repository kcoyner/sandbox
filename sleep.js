const axios = require('axios')

var instance = axios.create({
  baseURL: 'https://cp.remotethings.co.uk/api',
  timeout: 5000,
  headers: {'Authorization': 'pT4kQ4VifoHxQp6m27cbAbitEhs4Cs0zlE1KtV9Z9iv2r2871uT0QVarG1D991DC'}
})

instance.get('/devices/463/sleep').then((resp) => {
  console.log('resp.data: ', resp.data)
  console.log('resp.statusText: ', resp.statusText)
  console.log('resp.status: ', resp.status)
})
