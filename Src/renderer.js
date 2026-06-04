// ── State ─────────────────────────────────────────────
let allTracks     = []
let playlists     = []
let current       = null
let playing       = false
let position      = 0
let duration      = 0
let volume        = 0.8
let mode          = 'Normal'
let timer         = null
let activeTheme   = 'dark'
let selectedColor = '#7C4DFF'
let snapshotCount = 0
let obsLogOpen    = false
let snapshotsOpen = false

// ── Init ──────────────────────────────────────────────
async function init() {
  setupBar()
  const data = await window.api.getPlaylist()
  if (Array.isArray(data)) {
    allTracks = data
    renderTrackList()
  }
}

// ── Tab switching ─────────────────────────────────────
function switchSideTab(tab) {
  document.getElementById('side-tracks').style.display    = tab === 'tracks'    ? '' : 'none'
  document.getElementById('side-playlists').style.display = tab === 'playlists' ? '' : 'none'
  document.getElementById('tab-tracks').classList.toggle('active',    tab === 'tracks')
  document.getElementById('tab-playlists').classList.toggle('active', tab === 'playlists')
}

function switchMainTab(tab) {
  ;['player','patterns','theme','builder'].forEach(t => {
    document.getElementById(`view-${t}`).style.display = t === tab ? '' : 'none'
    document.getElementById(`mtab-${t}`).classList.toggle('active', t === tab)
  })
}

// ── STRATEGY: Sort playlist ───────────────────────────
async function sortPlaylist(sort) {
  const data = sort
    ? await window.api.getPlaylistSorted(sort)
    : await window.api.getPlaylist()

  if (Array.isArray(data)) {
    allTracks = data
    renderTrackList()
    document.getElementById('demo-strategy').textContent =
      `Strategy: SortBy${sort ? sort.charAt(0).toUpperCase()+sort.slice(1) : 'Default'}\n` +
      `→ ${allTracks.length} tracks sortate`
    showToast(`Sortat dupa: ${sort || 'default'}`)
  }
}

// ── Render track list ─────────────────────────────────
function renderTrackList() {
  const el = document.getElementById('track-list')
  if (!allTracks.length) {
    el.innerHTML = `<div style="padding:20px;text-align:center;color:var(--muted);font-size:12px;">
      Apasa + pentru a adauga muzica</div>`
    return
  }
  el.innerHTML = allTracks.map((t, i) => `
    <div class="pitem${current?.id === t.id ? ' active' : ''}"
         onclick='playTrack(${JSON.stringify(t).replace(/'/g,"&#39;")})'>
      <span class="pnum">${current?.id === t.id && playing ? '♫' : i + 1}</span>
      <div class="pinfo">
        <div class="ptitle">${esc(t.title)}</div>
        <div class="partist">${esc(t.artist || '')}</div>
      </div>
      <span class="pdur">${fmt(t.duration)}</span>
    </div>`).join('')
}

// ── Render playlist list ──────────────────────────────
function renderPlaylistList() {
  const el = document.getElementById('playlist-list')
  if (!playlists.length) {
    el.innerHTML = `<div style="padding:20px;text-align:center;color:var(--muted);font-size:12px;">
      Niciun playlist. Apasa ⊞</div>`
    return
  }
  el.innerHTML = playlists.map(pl => `
    <div class="pl-group">
      <div class="pl-header">
        <div class="pl-dot" style="background:${pl.color}"></div>
        <div class="pl-name">${esc(pl.name)}</div>
        <div class="pl-count">${pl.tracks.length} tracks</div>
      </div>
    </div>`).join('')
}

// ── Player commands ───────────────────────────────────
async function togglePlay() {
  if (playing) {
    const state = await window.api.pause()
    applyState(state)
    addObsEntry('pause', `Paused: ${current?.title || '—'}`)
  } else {
    const id = current?.id || allTracks[0]?.id
    if (!id) return
    const state = await window.api.play(id)
    applyState(state)
    startTimer()
    addObsEntry('play', `Playing: ${current?.title || '—'}`)
  }
  updateUndoBtn()
}

async function playTrack(track) {
  current  = track
  duration = track.duration || 0
  updateNowPlaying()

  const state = await window.api.play(track.id)
  applyState(state)
  startTimer()
  renderTrackList()
  renderPlaylistList()
  switchMainTab('player')
  addObsEntry('play', `Playing: ${track.title} — ${track.artist}`)
  updateUndoBtn()
}

async function sendCmd(action) {
  let state
  try {
    state = await window.api[action]()
  } catch(e) {
    console.error('sendCmd error:', action, e)
    return
  }
  if (!state) { console.warn('sendCmd no state:', action); return }

  const currentId = state.currentId ?? state.CurrentId ?? ''
  if (currentId) {
    const t = allTracks.find(x => x.id === currentId)
    if (t) { current = t; duration = t.duration || 0; updateNowPlaying() }
  }
  applyState(state)
  if (state.isPlaying ?? state.IsPlaying) startTimer()
  renderTrackList()
  addObsEntry('play', `${action}: ${current?.title || '—'}`)
  updateUndoBtn()
}

async function setVol(val) {
  volume = parseFloat(val) / 100
  updateVolIcon()
  try { await window.api.setVolume(volume) } catch(e) { console.error('setVolume error:', e) }
}

async function setMode(m) {
  const newMode = (mode === m) ? 'Normal' : m
  try {
    const state = await window.api.setMode(newMode)
    if (state) {
      const modeMap = { 0:'Normal', 1:'Shuffle', 2:'RepeatOne', 3:'RepeatAll' }
      const raw = state.mode ?? state.Mode ?? 0
      mode = typeof raw === 'number' ? (modeMap[raw] || 'Normal') : raw
    } else {
      mode = newMode
    }
  } catch(e) { mode = newMode }
  updateModeUI()
  showToast(`Mod: ${mode}`)
}

// ── COMMAND: Undo ─────────────────────────────────────
async function doUndo() {
  try {
    const state = await window.api.undo()
    if (!state) { showToast('Nimic de undo'); return }
    applyState(state)
    addObsEntry('stop', 'Undo executat')
    updateUndoBtn()
    showToast('↩ Undo!')
  } catch(e) { console.error('doUndo error:', e); showToast('Eroare la undo') }
}

async function updateUndoBtn() {
  try {
    const history = await window.api.getHistory()
    const btn = document.getElementById('btnUndo')
    if (Array.isArray(history) && history.length > 0) {
      btn.disabled = false
      btn.title = `Undo: ${history[0]}`
    } else {
      btn.disabled = true
    }
  } catch(e) { console.error('updateUndoBtn error:', e) }
}

async function loadHistory() {
  try {
    const history = await window.api.getHistory()
    const el = document.getElementById('demo-command')
    if (!Array.isArray(history) || !history.length) {
      el.textContent = 'History: gol\nExecuta Play/Pause/Skip'
      showToast('History gol')
      return
    }
    el.textContent = `History (${history.length}):\n` +
      history.map((h, i) => `${i+1}. ${h}`).join('\n')
    showToast(`${history.length} comenzi in history`)
  } catch(e) { console.error('loadHistory error:', e) }
}

// ── MEMENTO: Save / Restore ───────────────────────────
async function doSave() {
  try {
    const res = await window.api.saveState()
    if (!res) { showToast('Eroare la salvare'); return }
    snapshotCount = res.count ?? (snapshotCount + 1)
    document.getElementById('btnRestore').disabled = false
    document.getElementById('demo-memento').textContent =
      `Snapshot salvat!\nOra: ${new Date().toLocaleTimeString()}\nTotal: ${snapshotCount}`
    showToast(`Salvat! (${snapshotCount} snapshots)`)
  } catch(e) { console.error('doSave error:', e); showToast('Eroare la salvare') }
}

async function doRestore() {
  try {
    const state = await window.api.restoreState()
    if (!state) { showToast('Niciun snapshot disponibil'); return }
    applyState(state)
    showToast('⏪ Stare restaurata!')
    addObsEntry('stop', 'Memento restaurat')
    if (snapshotsOpen) toggleSnapshots()
  } catch(e) { console.error('doRestore error:', e); showToast('Eroare la restaurare') }
}

async function toggleSnapshots() {
  snapshotsOpen = !snapshotsOpen
  document.getElementById('snapshots-panel').classList.toggle('open', snapshotsOpen)
  if (snapshotsOpen) {
    try {
      const snapshots = await window.api.getSnapshots()
      const body = document.getElementById('snap-body')
      if (!Array.isArray(snapshots) || !snapshots.length) {
        body.innerHTML = `<div style="color:var(--muted);font-size:11px;font-family:'Space Mono',monospace;padding:4px">Niciun snapshot...</div>`
        return
      }
      body.innerHTML = snapshots.map((s, i) => `
        <div class="snap-item" onclick="restoreSnapshot()">
          [${s.savedAt}] ${s.isPlaying ? '▶' : '⏸'} Vol:${Math.round(s.volume*100)}%
          <br>Mode: ${s.mode}
        </div>`).join('')
    } catch(e) { console.error('toggleSnapshots error:', e) }
  }
}

async function restoreSnapshot() {
  try {
    const state = await window.api.restoreState()
    if (state) {
      applyState(state)
      snapshotsOpen = true
      toggleSnapshots()
      showToast('⏪ Snapshot restaurat!')
    }
  } catch(e) { console.error('restoreSnapshot error:', e) }
}

// ── OBSERVER: Log ─────────────────────────────────────
function toggleObsLog() {
  obsLogOpen = !obsLogOpen
  document.getElementById('obs-log').classList.toggle('open', obsLogOpen)
}

function addObsEntry(type, msg) {
  const body = document.getElementById('obs-log-body')
  const time = new Date().toLocaleTimeString()
  if (body.children.length === 1 && body.children[0].style.color) body.innerHTML = ''
  const entry = document.createElement('div')
  entry.className = `obs-entry ${type}`
  entry.textContent = `[${time}] ${msg}`
  body.insertBefore(entry, body.firstChild)
  while (body.children.length > 20) body.removeChild(body.lastChild)
  document.getElementById('demo-observer').textContent =
    `3 observeri activi:\nNowPlayingObserver\nPlayCountObserver\nLoggerObserver\n\nUltimul: ${msg}`
}

// ── FACTORY METHOD: Butonul + → modal cu 2 moduri ─────
// Apăsând + se deschide un modal care permite:
//   1. FileTrackFactory  — alege fișier real (Electron)
//   2. DemoTrackFactory  — adaugă track demo cu titlu custom

async function showAddFiles() {
  if (!window.electron) {
    showToast('File picker disponibil doar in Electron')
    return
  }
 
  // Electron dialog — selectează unul sau mai multe fișiere audio
  const paths = await window.electron.openFiles()
  if (!paths || !paths.length) return
 
  let added = 0
  for (const p of paths) {
    try {
      // POST /api/player/add → FileTrackFactory.Create(path) → ITrack
      const track = await window.api.addFile(p)
 
      if (track?.id ?? track?.Id) {
        // Normalizează PascalCase → camelCase
        const normalized = {
          id:        track.id        ?? track.Id,
          title:     track.title     ?? track.Title,
          artist:    track.artist    ?? track.Artist,
          album:     track.album     ?? track.Album,
          genre:     track.genre     ?? track.Genre,
          filePath:  track.filePath  ?? track.FilePath,
          duration:  track.duration  ?? track.Duration  ?? 0,
          playCount: track.playCount ?? track.PlayCount ?? 0,
          isFavorite:track.isFavorite?? track.IsFavorite?? false
        }
        allTracks.push(normalized)
        added++
 
    
        document.getElementById('demo-factory').textContent =
          `FileTrackFactory.Create(...)\n` +
          `→ ITrack returnat:\n` +
          `  Title:  "${normalized.title}"\n` +
          `  Artist: "${normalized.artist}"\n` +
          `  Path:   "${p.split(/[\\/]/).pop()}"`
      }
    } catch (e) {
      console.error('[showAddFiles] eroare:', e)
      showToast(`Eroare la: ${p.split(/[\\/]/).pop()}`)
    }
  }
 
  if (added > 0) {
    renderTrackList()
    showToast(`${added} track${added > 1 ? '-uri' : ''} adăugate`)
    addObsEntry('play', `FileTrackFactory: ${added} track${added > 1 ? '-uri' : ''} create`)
  }
}

function openFactoryModal() {
  const modal = document.getElementById('modal-factory')
  if (!modal) { _legacyAddFiles(); return }
  document.getElementById('factory-title-input').value = ''
  document.getElementById('factory-result').style.display = 'none'
  document.getElementById('factory-result').textContent = ''
  modal.classList.add('open')
  document.getElementById('factory-title-input').focus()
}

function closeFactoryModal() {
  document.getElementById('modal-factory').classList.remove('open')
}

// Opțiunea 1: FileTrackFactory prin Electron file picker
async function factoryFromFile() {
  if (!window.electron) {
    showToast('File picker disponibil doar in Electron')
    // Fallback in browser: adaugă demo cu titlu random
    await factoryAddDemo('Demo Track ' + Date.now())
    return
  }
  closeFactoryModal()
  const paths = await window.electron.openFiles()
  if (!paths || !paths.length) return
  let added = 0
  for (const p of paths) {
    try {
      const track = await window.api.addFile(p)
      if (track?.id) {
        allTracks.push(track)
        added++
        // Actualizează demo-factory în tab Patterns
        document.getElementById('demo-factory').textContent =
          `FileTrackFactory.Create("${p.split(/[\\/]/).pop()}")\n` +
          `→ ITrack returnat:\n` +
          `  Title:  "${track.title}"\n` +
          `  Artist: "${track.artist}"\n` +
          `  Genre:  "${track.genre}"\n` +
          `  Dur:    ${fmt(track.duration)}`
      }
    } catch(e) { console.error('addFile error:', e) }
  }
  renderTrackList()
  showToast(`${added} fișier(e) adăugate via FileTrackFactory`)
}

// Opțiunea 2: DemoTrackFactory — titlu din input
async function factoryAddDemoFromModal() {
  const input = document.getElementById('factory-title-input')
  const title = input.value.trim()
  if (!title) {
    input.style.borderColor = 'var(--pink)'
    input.focus()
    return
  }
  input.style.borderColor = ''
  await factoryAddDemo(title)
  closeFactoryModal()
}

async function factoryAddDemo(title) {
  try {
    const res = await window.api.addDemo(title)
    if (!res?.track) { showToast('Eroare la creare track demo'); return }

    const track = res.track
    // Normalizează câmpurile (ASP.NET returnează PascalCase)
    const normalized = {
      id:       track.id       ?? track.Id,
      title:    track.title    ?? track.Title,
      artist:   track.artist   ?? track.Artist,
      album:    track.album    ?? track.Album,
      genre:    track.genre    ?? track.Genre,
      filePath: track.filePath ?? track.FilePath,
      duration: track.duration ?? track.Duration ?? 0,
      playCount:track.playCount?? track.PlayCount?? 0,
      isFavorite:track.isFavorite??track.IsFavorite??false
    }

    allTracks.push(normalized)
    renderTrackList()

    // Actualizează demo-factory în tab Patterns
    document.getElementById('demo-factory').textContent =
      `DemoTrackFactory.Create("${normalized.title}")\n` +
      `→ ITrack returnat:\n` +
      `  Id:     ${normalized.id}\n` +
      `  Title:  "${normalized.title}"\n` +
      `  Artist: "${normalized.artist}"\n` +
      `  Dur:    ${fmt(normalized.duration)}\n\n` +
      `Factory: ${res.factoryUsed ?? 'DemoTrackFactory'}`

    showToast(`Factory Method → "${normalized.title}" adăugat`)
    addObsEntry('play', `Factory: creat "${normalized.title}"`)
  } catch(e) {
    console.error('factoryAddDemo error:', e)
    showToast('Eroare la DemoTrackFactory')
  }
}

// Fallback dacă modalul nu există în HTML
async function _legacyAddFiles() {
  await factoryAddDemo('Track ' + (allTracks.length + 1))
}

// ── PROTOTYPE: Clonare via API ────────────────────────
// Apelează POST /api/player/clone/{id} — backend face Track.Clone()
async function demoPrototype() {
  if (!current) { showToast('Selecteaza un track mai intai'); return }

  try {
    const res = await window.api.cloneTrack(current.id)
    if (!res?.clone) { showToast('Eroare la clonare'); return }

    const clone = {
      id:        res.clone.id    ?? res.clone.Id,
      title:     res.clone.title ?? res.clone.Title,
      artist:    current.artist,
      album:     current.album,
      genre:     current.genre,
      filePath:  current.filePath,
      duration:  current.duration,
      playCount: 0,
      isFavorite:false
    }

    allTracks.push(clone)
    renderTrackList()

    document.getElementById('demo-prototype').textContent =
      `Track.Clone()  ← Prototype pattern\n\n` +
      `Original: ${res.original?.id ?? current.id}\n` +
      `  Title:      "${res.original?.title ?? current.title}"\n` +
      `  PlayCount:  ${res.original?.playCount ?? current.playCount}\n` +
      `  IsFavorite: ${res.original?.isFavorite ?? current.isFavorite}\n\n` +
      `Clone:    ${clone.id}\n` +
      `  Title:      "${clone.title}"\n` +
      `  PlayCount:  0  ← resetat\n` +
      `  IsFavorite: false ← resetat`

    showToast(`Prototype → "${clone.title}" clonat`)
    addObsEntry('play', `Clone: "${clone.title}"`)
  } catch(e) {
    console.error('demoPrototype error:', e)
    showToast('Eroare la clonare')
  }
}

// ── Pattern demos (celelalte) ─────────────────────────
function demoFactory() {
  // Buton Demo din tab Patterns — deschide modalul
  openFactoryModal()
}

async function demoProxy() {
  const t1s = Date.now()
  await window.api.getPlaylist()
  const t1 = Date.now() - t1s
  const t2s = Date.now()
  await window.api.getPlaylist()
  const t2 = Date.now() - t2s
  document.getElementById('demo-proxy').textContent =
    `Prima cerere:  ${t1}ms → API call\nA doua cerere: ${t2}ms → faster\nCache entries: ${allTracks.length} tracks`
  showToast('Proxy cache testat!')
}

function demoFlyweight() {
  const groups = {}
  allTracks.forEach(t => {
    const key = `${t.genre}|${t.artist}`
    groups[key] = (groups[key] || 0) + 1
  })
  const lines = Object.entries(groups).map(([k, v]) => `${k} → ${v}x`).join('\n')
  document.getElementById('demo-flyweight').textContent =
    `Pool: ${Object.keys(groups).length} obiecte\n\n${lines}`
  showToast('Flyweight pool afisat!')
}

async function demoDecorator() {
  document.getElementById('demo-decorator').textContent =
    `[Log] Load: ${current?.title || '—'}\n[Log] Play\n` +
    `[VolumeNorm] ${volume.toFixed(2)} → ${Math.min(0.95,Math.max(0.0,volume)).toFixed(2)}\n` +
    `[FadeIn] vol: 0.0 → 0.5 → 1.0\n[FadeIn] Complet in 500ms`
  if (current) {
    const state = await window.api.play(current.id)
    applyState(state)
    startTimer()
    addObsEntry('play', `Decorator stack: ${current.title}`)
  }
  showToast('Decorator stack executat!')
}

async function demoStrategy(sort) {
  try {
    const data = await window.api.getPlaylistSorted(sort)
    if (Array.isArray(data)) {
      allTracks = data
      renderTrackList()
      document.getElementById('sort-select').value = sort
      document.getElementById('demo-strategy').textContent =
        `SortBy${sort.charAt(0).toUpperCase()+sort.slice(1)}\n` +
        `→ ${allTracks[0]?.title}\n→ ${allTracks[1]?.title}\n→ ...`
      showToast(`Strategy: sortare dupa ${sort}`)
    }
  } catch(e) { console.error('demoStrategy error:', e) }
}

async function demoBridge() {
  document.getElementById('demo-decorator').textContent =
    `AudioEngine (RefinedAbstraction)\n  extends AudioPlayer (Abstraction)\n` +
    `  delega la _backend.Play()\n  Backend: NAudioBackend (Implementor)\n` +
    `  Output: placa de sunet`
  if (current) {
    const state = await window.api.play(current.id)
    applyState(state)
    startTimer()
  }
  showToast('Bridge: AudioEngine → NAudioBackend')
}

async function demoIterator(shuffle) {
  try {
    const data = await window.api.getIterator(shuffle)
    if (!data) return
    document.getElementById('demo-iterator').textContent =
      `${shuffle ? 'ShuffleIterator' : 'PlaylistIterator'}\n` +
      `Count: ${data.count}\nIndex: ${data.currentIndex}\n\n` +
      (data.tracks || []).slice(0, 4).map((t, i) => `${i}. ${t.title}`).join('\n') +
      (data.count > 4 ? `\n...+${data.count - 4} more` : '')
    showToast(`Iterator ${shuffle ? 'Shuffle' : 'Normal'}: ${data.count} tracks`)
  } catch(e) { console.error('demoIterator error:', e) }
}

// ── Playlist modal ────────────────────────────────────
function showCreatePlaylist() {
  document.getElementById('modal-playlist').classList.add('open')
  document.getElementById('modal-name').focus()
}

function createPlaylist() {
  const name = document.getElementById('modal-name').value.trim()
  if (!name) {
    document.getElementById('modal-name').style.borderColor = 'var(--pink)'
    return
  }
  const pl = { id: Date.now().toString(), name, tracks: [], color: selectedColor }
  playlists.push(pl)
  closeModal('modal-playlist')
  switchSideTab('playlists')
  renderPlaylistList()
  showToast(`Playlist "${name}" creat`)
}

function closeModal(id) {
  document.getElementById(id).classList.remove('open')
}

// ── Theme (Abstract Factory) ──────────────────────────
function applyTheme(theme) {
  activeTheme = theme
  document.getElementById('theme-dark').classList.toggle('active',  theme === 'dark')
  document.getElementById('theme-light').classList.toggle('active', theme === 'light')
  const root = document.documentElement
  if (theme === 'dark') {
    root.style.setProperty('--bg',      '#080810')
    root.style.setProperty('--bg2',     '#0F0F1A')
    root.style.setProperty('--surface', '#13131F')
    root.style.setProperty('--purple',  '#7C4DFF')
    root.style.setProperty('--purple2', '#9C6FFF')
    root.style.setProperty('--white',   '#EEF0FF')
    root.style.setProperty('--muted',   '#6B6B8A')
    document.getElementById('theme-info').textContent =
      `Factory: DarkThemeFactory\nBackground: #080810\nAccent: #7C4DFF\nFont: Space Mono + DM Sans`
  } else {
    root.style.setProperty('--bg',      '#F5F5FA')
    root.style.setProperty('--bg2',     '#EBEBF5')
    root.style.setProperty('--surface', '#FFFFFF')
    root.style.setProperty('--purple',  '#5C35CC')
    root.style.setProperty('--purple2', '#7C55EE')
    root.style.setProperty('--white',   '#1A1A2E')
    root.style.setProperty('--muted',   '#8080A0')
    document.getElementById('theme-info').textContent =
      `Factory: LightThemeFactory\nBackground: #F5F5FA\nAccent: #5C35CC\nFont: Playfair Display + Source Sans`
  }
  showToast(`Tema ${theme === 'dark' ? 'Dark Neon' : 'Arctic Light'} aplicata!`)
}

// ── Builder ───────────────────────────────────────────
function selectColor(color) { selectedColor = color }

function buildPlaylist() {
  const name = document.getElementById('bl-name').value.trim()
  if (!name) {
    document.getElementById('bl-name').style.borderColor = 'var(--pink)'
    showToast('Numele este obligatoriu!')
    return
  }
  const modeVal = document.getElementById('bl-mode').value
  const desc    = document.getElementById('bl-desc').value.trim()
  const pl      = { id: Date.now().toString(), name, desc, mode: modeVal, color: selectedColor, tracks: [] }
  playlists.push(pl)
  renderPlaylistList()
  document.getElementById('builder-result').style.display = ''
  document.getElementById('builder-result').textContent =
    `Playlist construit!\n\nPlaylistBuilder\n` +
    `  .WithName("${name}")\n  .WithDescription("${desc || '—'}")\n` +
    `  .WithMode(${modeVal})\n  .WithColor("${selectedColor}")\n  .Build()\n\n→ ID: ${pl.id}`
  showToast(`Playlist "${name}" construit!`)
  switchSideTab('playlists')
}

function resetBuilder() {
  document.getElementById('bl-name').value = ''
  document.getElementById('bl-desc').value = ''
  document.getElementById('bl-mode').value = 'Normal'
  document.getElementById('builder-result').style.display = 'none'
}

// ── UI Updates ────────────────────────────────────────
function updateNowPlaying() {
  if (!current) {
    document.getElementById('empty').style.display      = ''
    document.getElementById('nowplaying').style.display = 'none'
    return
  }
  document.getElementById('empty').style.display      = 'none'
  document.getElementById('nowplaying').style.display = ''
  document.getElementById('mtitle').textContent  = current.title  || '—'
  document.getElementById('martist').textContent = current.artist || '—'
  document.getElementById('mgenre').textContent  = current.genre  || '—'
  document.getElementById('tdur').textContent    = fmt(current.duration)
  const icons = {
    Rock:'🎸', Jazz:'🎺', Pop:'🎤', Classical:'🎻',
    Electronic:'🎛', Grunge:'⚡', 'R&B':'🎷', Folk:'🪕'
  }
  document.getElementById('art').textContent = icons[current.genre] || '🎵'
}

function applyState(s) {
  if (!s) return
  playing  = s.isPlaying  ?? s.IsPlaying  ?? false
  position = s.position   ?? s.Position   ?? 0
  volume   = s.volume     ?? s.Volume     ?? 0.8
  const modeMap = { 0:'Normal', 1:'Shuffle', 2:'RepeatOne', 3:'RepeatAll' }
  const rawMode = s.mode ?? s.Mode ?? 0
  mode = typeof rawMode === 'number' ? (modeMap[rawMode] || 'Normal') : rawMode
  const currentId = s.currentId ?? s.CurrentId ?? ''
  if (currentId && (!current || current.id !== currentId)) {
    const t = allTracks.find(x => x.id === currentId)
    if (t) { current = t; duration = t.duration || 0; updateNowPlaying() }
  }
  document.getElementById('playbtn').textContent = playing ? '⏸' : '▶'
  document.getElementById('playbtn').classList.toggle('playing', playing)
  document.getElementById('playerbar').classList.toggle('playing', playing)
  const art = document.getElementById('art')
  if (art) art.classList.toggle('spin', playing)
  const volSlider = document.getElementById('vol')
  if (volSlider) volSlider.value = Math.round(volume * 100)
  updateVolIcon()
  updateModeUI()
  updateProgress()
}

function updateProgress() {
  const pct = duration > 0 ? (position / duration) * 100 : 0
  document.getElementById('fill').style.width = pct + '%'
  document.getElementById('tpos').textContent = fmt(position)
}

function updateModeUI() {
  const modeMap = { 0:'Normal', 1:'Shuffle', 2:'RepeatOne', 3:'RepeatAll' }
  const modeStr = typeof mode === 'number' ? (modeMap[mode] || 'Normal') : (mode || 'Normal')
  document.getElementById('modebadge').textContent =
    modeStr.replace('RepeatAll','REPEAT').replace('RepeatOne','REPEAT 1').toUpperCase()
  document.getElementById('btnShuffle').classList.toggle('on', modeStr === 'Shuffle')
  document.getElementById('btnRepeat').classList.toggle('on',  modeStr.startsWith('Repeat'))
  mode = modeStr
}

function updateVolIcon() {
  const el = document.getElementById('vicon')
  el.textContent = volume === 0 ? '🔇' : volume < 0.4 ? '🔈' : volume < 0.7 ? '🔉' : '🔊'
}

// ── Timer ─────────────────────────────────────────────
function startTimer() {
  clearInterval(timer)
  timer = setInterval(() => {
    if (!playing) return
    position++
    updateProgress()
    if (position >= duration && duration > 0) {
      clearInterval(timer)
      sendCmd('next')
    }
  }, 1000)
}

function setupBar() {
  document.getElementById('bar').addEventListener('click', async e => {
    if (!current) return
    const rect = document.getElementById('bar').getBoundingClientRect()
    position   = Math.floor(((e.clientX - rect.left) / rect.width) * duration)
    updateProgress()
    try { await window.api.seek(position) } catch(e) { console.error('seek error:', e) }
  })
}

// ── Toast ─────────────────────────────────────────────
function showToast(msg) {
  const el = document.getElementById('toast')
  el.textContent = msg
  el.classList.add('show')
  setTimeout(() => el.classList.remove('show'), 2500)
}

// ── Helpers ───────────────────────────────────────────
const fmt = s => `${Math.floor((s||0)/60)}:${String((s||0)%60).padStart(2,'0')}`
const esc = s => (s||'').replace(/&/g,'&amp;').replace(/</g,'&lt;')