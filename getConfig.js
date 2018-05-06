const axios = require('axios')

var instance = axios.create({
  baseURL: 'https://cp.remotethings.co.uk/api',
  timeout: 5000,
  headers: {'Authorization': 'PJeoXCVNAZoOI4K8f0o577IckQirhQmETUwqf85nfpo8Ndkc7n43mPDjSSlfuDsK'},
})

instance.get('/devices/463/config').then((resp) => {
  console.log('resp.data: ', resp.data)
  console.log('resp.statusText: ', resp.statusText)
  console.log('resp.status: ', resp.status)
}).catch((error) => console.log(error))
