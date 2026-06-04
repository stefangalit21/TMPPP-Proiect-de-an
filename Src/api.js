const BASE = 'http://localhost:5000/api/player'

async function get(endpoint) {
  const r = await fetch(`${BASE}/${endpoint}`)
  return r.json()
}

async function post(endpoint, body = {}) {
  const r = await fetch(`${BASE}/${endpoint}`, {
    method:  'POST',
    headers: { 'Content-Type': 'application/json' },
    body:    JSON.stringify(body)
  })
  return r.json()
}

async function del(endpoint) {
  const r = await fetch(`${BASE}/${endpoint}`, { method: 'DELETE' })
  return r.json()
}

window.api = {
  // Playback
  getState:         ()              => get('state'),
  play:             (id)            => post('play',     { id }),
  pause:            ()              => post('pause'),
  stop:             ()              => post('stop'),
  next:             ()              => post('next'),
  previous:         ()              => post('previous'),
  setVolume:        (value)         => post('volume',   { value }),
  setMode:          (mode)          => post('mode',     { mode }),
  seek:             (position)      => post('seek',     { position }),
  undo:             ()              => post('undo'),
  getHistory:       ()              => get('history'),

  // Playlist
  getPlaylist:      ()              => get('playlist'),
  getPlaylistSorted:(sort)          => get(`playlist?sort=${sort}`),
  removeTrack:      (id)            => del(`track/${id}`),

  // Factory Method — fișier real (Electron file picker)
  addFile:          (path)          => post('add',      { path }),

  // Factory Method — track demo (butonul + din browser / fără Electron)
  addDemo:          (title)         => post('add-demo', { title }),

  // Prototype — clonează un track existent
  cloneTrack:       (id)            => post(`clone/${id}`),

  // Memento
  saveState:        ()              => post('save'),
  restoreState:     ()              => post('restore'),
  getSnapshots:     ()              => get('snapshots'),

  // Iterator
  getIterator:      (shuffle=false) => get(`iterator?shuffle=${shuffle}`),
  iteratorNext:     ()              => post('iterator/next'),
  iteratorPrevious: ()              => post('iterator/previous'),
  iteratorReset:    ()              => post('iterator/reset'),
}