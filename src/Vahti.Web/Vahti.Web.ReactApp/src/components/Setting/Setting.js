import './Setting.css';

const Setting = (props) => {        
    return (
        <div className="Setting">        
            <input type="checkbox" checked={props.isChecked} onChange={props.visibilitySettingChanged}></input>
            <label className="Setting__text" key={props.id}>{props.name}</label>            
        </div>
    );
}

export default Setting;