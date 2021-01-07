import React, { Component } from 'react'
import ImageDataAccess from '../util/data_access/ImageDataAccess'
import TagDataAccess from '../util/data_access/TagDataAccess'
import Images from '../components/Images/Images'


class Home extends Component {
    constructor(props) {
        super(props)
        this.state = {
            Images: [],
            tagOptions: [],
            search: {
                loadtags: false,
                name: "",
                description: "",
                tagName: ""
            },
            loading: true
        }
        this.populateImages = this.populateImages.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.removeImage = this.removeImage.bind(this);

        this.ImageAccess = new ImageDataAccess();
        this.TagAccess = new TagDataAccess();
    }

    componentDidMount() {
        this.populateTags();
        this.populateImages();
    }

    render() {

        
        if (this.state.loading) {
            return (
                <div>
                    <FilterBar handleSubmit={this.handleSubmit} handleChange={this.handleChange}
                        tagOptions={this.state.tagOptions} tagName={this.state.search.tagName} />
                    <hr/>
                    <p className="text-center"> Loading... </p>
                </div>
            );

        }
        else if (this.state.Images.length > 0) {
            return (
                <div className="">
                    <FilterBar handleSubmit={this.handleSubmit} handleChange={this.handleChange}
                        tagOptions={this.state.tagOptions} tagName={this.state.search.tagName} />
                    <hr />
                    <Images cards={this.state.Images} deleteImage={this.removeImage} />
                </div>
            );
        }
        else {
            return (
                <div>
                    <FilterBar handleSubmit={this.handleSubmit} handleChange={this.handleChange}
                        tagOptions={this.state.tagOptions} tagName={this.state.search.tagName} />
                    <hr/>
                    <p className="text-center"> No results found </p>
                </div>
                );
        }

    }

    async populateTags() {
        const tags = await this.TagAccess.getDistinctTags();
        await this.setState({ ['tagOptions']: tags });
    }

    async populateImages() {
        var images = await this.ImageAccess.getImages(
            this.state.search.loadtags,
            this.state.search.name,
            this.state.search.description,
            this.state.search.tagName);
            
        this.setState({ ['Images']: images, ['loading']: false });
    }

    async removeImage(e, id) {
        e.preventDefault();
        this.ImageAccess.deleteImage(id);

        this.setState({ ['loading']: true });

        this.populateImages()
    }

    handleChange(e) {
        let nam = e.target.name;
        let val = e.target.value;
        var search = this.state.search

        if (nam === "tagName" && val === "~Tag Search~") {
            val = ""
        }

        search[nam] = val
    }

    async handleSubmit(e) {
        e.preventDefault();
        this.setState({ ['loading']: true });
        await this.populateImages();
    }
} 

function FilterBar(props) {
    const tags = Array.from(props.tagOptions);
    tags.splice(0, 0, "~Tag Search~");

    return (
        <form className="form-inline p-3 row" onSubmit={props.handleSubmit}>
                <input className="form-control col-md-4 col-12 mr-2 mb-2" type="search" placeholder="Name" name="name" aria-label="Search" onChange={props.handleChange} />
                <input className="form-control col-md-4 mr-2 col-12 mb-2" type="search" placeholder="Description" name="description" aria-label="Search" onChange={props.handleChange} />
                <select className="custom-select col-md-2 col-12 mr-2 mb-2" name="tagName" id="tagName" onChange={props.handleChange}>
                    {tags.map(tag => {
                        if (tag === props.tagName) {
                            return <option selected key={ tag} value={tag}>{tag}</option>
                        }
                        else {
                            return <option key={tag} value={tag}>{tag}</option>
                        }
                    })}
                </select>
            <button type="submit" className="btn btn-outline-success col-md-1 mb-2 offset-md-0 offset-6 col-6">search</button>
        </form>
        );
}

export default Home;