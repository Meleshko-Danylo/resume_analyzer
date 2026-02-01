import React from 'react';
import '../App.css';
import Header from "../Components/Header";
import ResumeContainer, {AnalyzerType} from "../Components/ResumeContainer";
import AnalyzerResults from "../Components/AnalyzerResults";

function App() {

    return (
        <div className="App-wrapper">
            <Header/>
            <div className="App flex-col xl:flex-row xl:h-[90vh]">
                <ResumeContainer analyzer_type={AnalyzerType.Standard}/>
                <AnalyzerResults />
            </div>
        </div>
    );
}

export default App;
