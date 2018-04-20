const axios = require('axios')

var instance = axios.create({
  baseURL: 'https://cp.remotethings.co.uk/api',
  timeout: 5000,
  headers: {
    Authorization:
      'pT4kQ4VifoHxQp6m27cbAbitEhs4Cs0zlE1KtV9Z9iv2r2871uT0QVarG1D991DC'
  }
})

const body = {
  currentFW: '44',
  otaFW: null,
  interval: 30,
  sleepInterval: 3600,
  checkInInterval: 43500,
  packing: 1,
  movementSensitivity: 1,
  debounce: 2,
  movementSensitivity2: 1,
  behavior: 0,
  modeControl: 0,
  gpsTimeout: 90,
  transmitTimeout: 0,
  gpsStabilize: 10,
  gpsCheckInterval: 300,
  stopTimeout: 90,
  tolerancePercentage: 3,
  reasonsToWake: [{ bluetooth: false, move: true, geofence: false }],
  modified: '2018-04-20T22:13:13.871Z',
  forceFw: false,
  receivedAt: '2018-04-19T22:03:46.000Z',
  reset: null,
  flashTryCount: 0,
  homeWifiNetwork: 'station4-1042',
  wakeAction: 'normal',
  onDemandTime: 30,
  alertAction: 'nothing',
  id: 456,
  deviceId: 463,
  safeZoneId: null
}

instance.put('/devices/463/config', body).then(resp => {
  console.log('resp.data: ', resp.data)
  console.log('resp.statusText: ', resp.statusText)
  console.log('resp.status: ', resp.status)
})
