import React from 'react';
import '../App.css';
import Header from "../Components/Header";
import ResumeContainer from "../Components/ResumeContainer";
import AnalyzerResults from "../Components/AnalyzerResults";

function App() {

    return (
        <div className="App-wrapper">
            <Header/>
            <div className="App h-[85vh]">
                <ResumeContainer />
                <AnalyzerResults />
            </div>
        </div>
    );
}

export default App;
