const { app, BrowserWindow, ipcMain, dialog } = require('electron')
const path  = require('path')
const spawn = require('child_process').spawn

let win
let apiProcess

function startApi() {
  const isWin = process.platform === 'win32'
  const exe   = path.join(__dirname, '..', '..', 'bin', 'Release', 'net8.0',
    'MusicPlayer.Web' + (isWin ? '.exe' : ''))

  // Verificam daca exe-ul exista inainte sa il pornim
  const fs = require('fs')
  if (!fs.existsSync(exe)) {
    console.warn('[Electron] API negasit, presupunem ca ruleaza deja pe :5000')
    return
  }

  try {
    apiProcess = spawn(exe, [], { stdio: 'pipe' })
    apiProcess.stdout.on('data', d => console.log('[API]', d.toString().trim()))
    apiProcess.stderr.on('data', d => console.log('[API]', d.toString().trim()))
    apiProcess.on('exit', code => console.log('[API] Oprit, cod:', code))
  } catch (e) {
    console.warn('[Electron] Eroare la pornire API:', e.message)
  }
}

ipcMain.handle('open-files', async () => {
  const r = await dialog.showOpenDialog(win, {
    properties: ['openFile', 'multiSelections'],
    filters: [{ name: 'Audio', extensions: ['mp3', 'flac', 'wav', 'ogg'] }]
  })
  return r.filePaths
})

ipcMain.on('win-min',   () => win.minimize())
ipcMain.on('win-close', () => { apiProcess?.kill(); app.quit() })

app.whenReady().then(() => {
  startApi()

  win = new BrowserWindow({
    width:  1100,
    height: 720,
    minWidth:  800,
    minHeight: 600,
    frame: false,
    backgroundColor: '#080810',
    webPreferences: {
      preload:          path.join(__dirname, 'preload.js'),
      contextIsolation: true
    }
  })

  win.loadFile(path.join(__dirname, 'index.html'))
})

app.on('window-all-closed', () => {
  apiProcess?.kill()
  app.quit()
})
app.whenReady().then(() => {
  startApi()

  win = new BrowserWindow({
    width:  1100,
    height: 720,
    minWidth:  800,
    minHeight: 600,
    frame: false,
    backgroundColor: '#080810',
    webPreferences: {
      preload:          path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      webSecurity:      false
    }
  })

  win.loadFile(path.join(__dirname, 'index.html'))
  win.webContents.openDevTools()  // ← adauga asta
})
