import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faLongArrowAltLeft } from '@fortawesome/free-solid-svg-icons';
import GraphList from '../GraphList/GraphList';
import './Graphs.css';

const Graphs = (props) => {
    return (
        <div>      
            <header className="ChildPage__header">                    
              <div className="ChildPages__header__title">
                <a href="/">
                  <FontAwesomeIcon icon={faLongArrowAltLeft} size="2x"/>
                </a>
                <h2 className="ChildPage__header_title_text">Graphs</h2>
              </div>
            </header>
            <div className="GraphList__body"> 
              <GraphList location={props.location}/>
            </div>      
          </div>
    );
}

export default Graphs;