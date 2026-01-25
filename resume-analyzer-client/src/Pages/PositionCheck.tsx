import React, {useState} from 'react';
import Header from "../Components/Header";
import ResumeContainer from "../Components/ResumeContainer";
import AnalyzerResults from "../Components/AnalyzerResults";
import {Response} from "../Core/Response";

const PositionCheck = () => {
    const [analysisResults, setAnalysisResults] = useState<Response | null>(null);
    
    return (
        <div className="App-wrapper">
            <Header/>
            <div className="App h-[85vh]">
                <ResumeContainer onAnalysisComplete={setAnalysisResults}/>
                <AnalyzerResults results={analysisResults}/>
            </div>
        </div>
    );
};

export default PositionCheck;