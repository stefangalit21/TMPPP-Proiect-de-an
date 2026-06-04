const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld('electron', {
  openFiles: () => ipcRenderer.invoke('open-files'),
  minimize:  () => ipcRenderer.send('win-min'),
  close:     () => ipcRenderer.send('win-close'),
})