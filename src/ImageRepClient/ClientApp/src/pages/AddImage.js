import React, { Component } from 'react'
import { v4 as uuidv4 } from 'uuid';
import ImageDataAccess from '../util/data_access/ImageDataAccess'
import TagGroup from '../components/Tags/TagGroup';
import TagFrom from '../components/Tags/TagForm';

class AddImage extends Component {
    constructor() {
        super()
        this.state = {
            image: {
                Id: uuidv4(),
                Name: "",
                Description: "",
                Picture: "",
                Tags: []
            },
            imageLoaded: false,
            acceptableFileTypes: []

        }
        this.ImageDataAccess = new ImageDataAccess();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleNewTag = this.handleNewTag.bind(this);
        this.handleDescriptionChange = this.handleDescriptionChange.bind(this);
        this.handleFileUpdate = this.handleFileUpdate.bind(this);
        this.handleRemoveTag = this.handleRemoveTag.bind(this);
        this.loadLegalFileExtensions = this.loadLegalFileExtensions.bind(this);

    }
    
    async componentDidMount(){
        await this.loadLegalFileExtensions()
    }
    
    async loadLegalFileExtensions(){
        let fileList = await this.ImageDataAccess.getLegalFileExtensions();
        this.setState({acceptableFileTypes: fileList})

    }

    render() {
        return (
            <>
                <form onSubmit={this.handleSubmit}>
                    <div class="form-group">
                        <label for="imageSelectInput" className="font-weight-bold">Upload File ({this.state.acceptableFileTypes.map(str => str + ", ")})</label>
                        <input type="file" className="form-control-file" id="imageSelectInput" onChange={this.handleFileUpdate} />
                    </div>
                    <div class="form-group">
                        <label for="descriptionTextArea" className="font-weight-bold">Description</label>
                        <textarea className="form-control" name="Description" onChange={this.handleDescriptionChange} id="descriptionTextArea" rows="4"></textarea>
                    </div>
                        {
                        this.state.imageLoaded?(
                            <div className="form-group">
                                
                                <label for="newTagInput" className="font-weight-bold">Tags</label><br />
                                <TagGroup tags={this.state.image.Tags} handleRemoveTag={this.handleRemoveTag} deleteButton={true}/>
                                <TagFrom ImageKey={this.state.image.Id} handleAddTag={this.handleNewTag} /> 
                                
                            </div>
                        ):(
                            <></>
                        )
                        }
                        <div className="row">
                            <div className="col-6 offset-md-4 col-md-4 form-group">
                                <a href="/" className="btn btn-outline-primary btn-block">Return to Home</a>
                            </div>
                            <div className="col-6 col-md-4 pl-0 form-group">
                                <button type="submit" className="btn btn-outline-success btn-block">Sumbit</button>
                            </div>
                        </div>
                </form>
               
                
             </>
        );
    }

    async handleSubmit(e) {
        e.preventDefault();
        if(this.state.imageLoaded){
            await this.ImageDataAccess.addImage(this.state.image)
    
            let image = this.state.image;
            image.Id = uuidv4()
            image.Tags = []
            this.setState({ ['image']: image })
        }
        else{
            alert("Please upload an image")
        }
    }

    handleNewTag(e, tag) {
        let image = this.state.image;
        let add = true;
        
        image.Tags.forEach(item => {
            if(item.name === tag.name){
                add=false
            }
        });
        if(tag.Name !== "" && add){
            
            image.Tags.push(tag)
            this.setState({['image']: image})
        }
    }

    handleRemoveTag(e, id) {
        e.preventDefault();

        let image = this.state.image;

        image.Tags = image.Tags.filter(tag => tag.tagId !== id)

        this.setState({['image']: image})
    }

    handleDescriptionChange(e) {
        let image = this.state.image;
        image.Description = e.target.value
    }

    handleFileUpdate(e) {
        let file = null
        try{
            file = e.target.files[0] || e.dataTransfer.files[0]
        }
        catch(err){
            return false;
        }
        this.setState({['imageLoaded']: false})


        let reader = new FileReader();
        if (file) {

            if (!this.checkFileType(file.name)) {
                alert("File type is not valid");
                e.target.value = '';
                return false;
            }

            reader.readAsDataURL(file);
            reader.onload = () => {
                let Base64 = reader.result;

                let image = this.state.image;
                image.Picture = Base64.substring(23)
                image.Name = file.name

                this.setState({ ['image']: image, ['imageLoaded']: true})
            };
            reader.onerror = (error) => {
                console.log("error: ", error);
            };
        }
    }

    checkFileType(fileName) {
        
        let extension = fileName.substring(fileName.lastIndexOf("."))

        return this.state.acceptableFileTypes.includes(extension);
    }
}

export default AddImage