import React, { Component } from 'react'
import TagGroup from '../components/Tags/TagGroup'
import TagForm from '../components/Tags/TagForm'
import ImageDataAccess from '../util/data_access/ImageDataAccess'

class EditImage extends Component {
    constructor({match}) {
        super()
        this.match = match
        this.ImageDataAccess = new ImageDataAccess();

        this.state = {
            editMode: false,
            loading: true,
            image: {
                id: "",
                name: "",
                description: "",
                picture: "",
                tags: []
            },
        }
        
        this.populateImageData = this.populateImageData.bind(this);
        this.editMode = this.editMode.bind(this);
        this.openImage = this.openImage.bind(this);
        this.updateName = this.updateName.bind(this);
        this.updateDescription = this.updateDescription.bind(this);
        this.handleAddTag = this.handleAddTag.bind(this);
        this.handleRemoveTag = this.handleRemoveTag.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.formatDate = this.formatDate.bind(this);
    }

    componentDidMount() {
        this.populateImageData();
    }

    async populateImageData(){
        let Image = await this.ImageDataAccess.getImage(this.match.params.id, true);
        this.setState({image: Image, ['loading']: false})
    }

    editMode(e){
        this.setState({['editMode']: e.target.checked})
    }



    render() {
        if(this.state.loading === true){
            return(
                <p className="text-center"> Loading... </p>
            )
        } 
        else if(this.state.image === null){
        return (
            <p className="text-center"> There was an error loading your image. Please return to the previous page and try again. </p>
            ) 
        }
        else{
            return(
                <form onSubmit={this.handleSubmit}>
                    <div className="row">
                        <div className="col-12 mb-1 col-md-3">
                            <a href={"data:image/" + this.state.image.name.substring(this.state.image.name.lastIndexOf(".") + 1)+ ";base64," + this.state.image.picture} className="btn btn-primary btn-block" onClick={this.openImage}>View Full Image</a>
                        </div>
                        <div className="col-12 mb-1 col-md-3">
                            <a href={"data:" + this.state.image.name.substring(this.state.image.name.lastIndexOf(".") + 1) + "/jpg;base64," + this.state.image.picture} download={this.state.image.name} className="btn btn-secondary btn-block" onClick={this.openImage}>Download Image</a>
                        </div>
                        <div className="custom-control custom-switch col-12 col-md-3 offset-md-3 text-right">
                            <input type="checkbox" className="custom-control-input" onChange={this.editMode} id="editModeSwitch"/>
                            <label className="custom-control-label" htmlFor="editModeSwitch">Edit Mode</label>
                        </div>          
                    </div>
                    <hr/>
                    <div className="form-group col-12 col-md-6 p-0">
                        <label htmlFor="nameInput" className="font-weight-bold">Name</label>
                        <div className="input-group mb-3">
                            <input type="text" className="form-control" name="Name" id="nameInput" value={this.state.image.name.substring(0, this.state.image.name.lastIndexOf("."))} onChange={this.updateName} disabled={!this.state.editMode} aria-describedby="fileName"/> 
                            <div className="input-group-append">
                                <span className="input-group-text" id="fileName">{this.state.image.name.substring(this.state.image.name.lastIndexOf("."))}</span>
                            </div>
                        </div>
                    </div>
                    <div className="form-group">
                        <label htmlFor="descriptionTextArea" className="font-weight-bold">Description</label>
                        <textarea className="form-control" name="Description" onChange={this.handleDescriptionChange} id="descriptionTextArea" rows="4" disabled={!this.state.editMode} value={this.state.image.description}></textarea>
                    </div>
                    <div className="form-group col-12 col-md-6 p-0">
                        <label htmlFor="dateInput" className="font-weight-bold">Date Created</label>
                        <input type="text" className="form-control" name="Name" id="dateInput" value={this.formatDate(this.state.image.dateCreated)} disabled/> 
                    </div>
                    <div className="form-group">
                        <TagGroup tags={this.state.image.tags} handleRemoveTag={this.handleRemoveTag} deleteButton={this.state.editMode}/>
                            {
                                this.state.editMode?(
                                    <>
                                    <TagForm ImageKey={this.state.image.id} handleAddTag={this.handleAddTag} />
                                    </>
                                ):(
                                    <></>
                                )
                            }
                    </div>
                    <div className="form-group row">
                    {
                                this.state.editMode?(
                                    <>
                                        <div className="offset-0 col-12 offset-md-6 col-md-3">
                                            <a href="/" className="btn btn-outline-primary btn-block">Return to Home</a>
                                        </div>
                                        <div className="col-12 col-md-3">
                                            <input type="submit" className="btn btn-outline-success btn-block" value="Submit" />
                                        </div>
                                    </>
                                ):(
                                    <>
                                        <div className="offset-0 col-12 offset-md-9 col-md-3">
                                            <a href="/" className="btn btn-outline-primary btn-block">Return to Home</a>
                                        </div>
                                    </>
                                )
                            }
                        
                    </div>
                </form>
            )
        }
        
    }

    openImage () {
        var image = new Image();
        image.src = "data:image/jpg;base64," + this.state.image.picture;

        var w = window.open("");
        w.document.write(image.outerHTML);
    }

    updateName(e){
        let image = this.state.image
        let extension = this.state.image.name.substring(this.state.image.name.lastIndexOf("."));
        image.name = e.target.value + extension

        this.setState({image: image})
    }

    updateDescription(e){
        let image = this.state.image
        image.description = e.target.value

        this.setState({image: image})
    }

    handleAddTag(e, tag) {
        let image = this.state.image;
        let add = this.state.editMode;
        

        image.tags.forEach(item => {
            if(item.name === tag.name){
                add=false
            }
        });

        if(tag.Name !== "" && add){
            image.tags.push(tag)
            this.setState({['image']: image})
        }
    }

    handleRemoveTag(e, id) {
        e.preventDefault();

        let image = this.state.image;

        image.tags = image.tags.filter(tag => tag.tagId !== id)

        this.setState({['image']: image})
    }

    handleSubmit(e){
        e.preventDefault();

        if(this.state.image.name.lastIndexOf(".") === 0){
            alert("Please enter a valid name");
            return false;
        }
        this.ImageDataAccess.patchImage(this.state.image, true);
    }


    formatDate(date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();
    
        if (month.length < 2) 
            month = '0' + month;
        if (day.length < 2) 
            day = '0' + day;
    
        return [year, month, day].join('-');
    }

    
}

export default EditImage