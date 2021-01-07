import React, {Component} from 'react'
import { v4 as uuidv4 } from 'uuid';

class TagForm extends Component{
    constructor(props){
        super(props)

        this.state = {
            name: "",
        }
        this.handleNameChange = this.handleNameChange.bind(this)
        this.handleSubmit = this.handleSubmit.bind(this)
    }


    render(){
        return (
            <div>
                <input type="text" name="Name" className="form-control-sm mb-2 mr-sm-2" size="6" placeholder="Tag Name" id="newTagInput" onChange={this.handleNameChange} />
                <button type="submit" className="btn btn-primary btn-sm mb-1" onClick={this.handleSubmit}>Submit</button>
            </div>
           );
    }

    handleSubmit(e) {
        
        e.preventDefault();
        const tag = this.state;
        const id = uuidv4();
        tag.tagId = id;
        tag.key = id;
        tag.imageKey = this.props.ImageKey;
        this.props.handleAddTag(e, tag);
    }

    handleNameChange(e){
        this.setState({ ['name']: e.target.value });
    }
}

export default TagForm