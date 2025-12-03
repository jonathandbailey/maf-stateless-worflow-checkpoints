import './App.css'
import RootLayout from './components/layout/RootLayout'
import streamingService from './services/streaming.service';

streamingService.initialize();

function App() {

  return (
    <>
      <RootLayout />
    </>
  )
}

export default App
