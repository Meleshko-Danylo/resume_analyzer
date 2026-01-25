import React from 'react';
import { Link } from 'react-router';

const Header = () => {
    return (
        <div className={"header-container"}>
            <Link to={"/"}>Home</Link>
            <Link to={"/position-check"}>Position Check</Link>
            <Link to={"/resume-example"}>Resume example</Link>
        </div>
    );
};

export default Header;