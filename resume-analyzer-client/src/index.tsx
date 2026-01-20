import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './Pages/App';
import PositionCheck from "./Pages/PositionCheck";
import reportWebVitals from './reportWebVitals';
import { BrowserRouter as Router, Routes, Route } from "react-router";
import axios from "axios";

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

export const axiosInstance = axios.create({
    baseURL: process.env.REACT_APP_BASE_URL,
    // headers: {
    //     'Content-Type': 'application/json',
    //     Accept: 'application/json'
    // }
}) 

root.render(
  <Router>
      <Routes>
          <Route path="/" element={<App />} />
          <Route path="/position-check" element={<PositionCheck />} />
      </Routes>
  </Router>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
