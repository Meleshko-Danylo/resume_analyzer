import React from 'react';
import Header from "../Components/Header";
import ResumeContainer from "../Components/ResumeContainer";
import AnalyzerResults from "../Components/AnalyzerResults";

const PositionCheck = () => {

    return (
        <div className="App-wrapper">
            <Header/>
            <div className="App h-[85vh]">
                <ResumeContainer />
                <AnalyzerResults />
            </div>
        </div>
    );
};

export default PositionCheck;