import Image from "next/image";
import seallLogo from '../images/seall.avif';
import keerynPhoto from '../images/KeerynJohnson.jpg';
import ronniePhoto from '../images/SouzaSantos.jpg';

interface AboutUsModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export default function AboutUsModal({ isOpen, onClose }: AboutUsModalProps) {
  if (!isOpen) return null;

  return (
    <div className="info-modal-overlay" onClick={onClose}>
      <div className="info-modal" onClick={(e) => e.stopPropagation()}>
        <h2>About Us</h2>

        <div className="about-section">
          <Image
            src={seallLogo}
            alt="SEALL Logo"
            width={200}
            height={200}
            className="about-img"
          />
          <div>
            <h4>SE-ALL (Software Engineering for All Lab)</h4>
            <p>
              The SE-ALL (Software Engineering for All Lab) focuses 
              on the human aspects of software engineering, including development practices, 
              project management, software testing, fairness, and EDI. Understanding behaviors, 
              cognitive skills, teamwork, and diverse user perspectives is vital for creating 
              effective and innovative technology. As society becomes increasingly reliant on 
              software across work, education, politics, and leisure, and with the rise of 
              AI-powered systems, ensuring fairness and bias-free solutions in software is essential.
            </p>
          </div>
        </div>

        {/* Team Members */}
        <div className="about-section">
          <Image
            src={keerynPhoto}
            alt="Keeryn Johnson Portrait"
            width={200}
            height={200}
            className="about-img"
          />
          <div>
            <h4>Keeryn Johnson (Undergraduate Research Assistant)</h4>
            <p>
              Keeryn Johnson is pursuing a Software Engineering degree with a minor in Mechatronics 
              at the University of Calgary. He is interested in robotics, interactive software design, 
              and the development of educational simulations that integrate real-world decision-making 
              and systems thinking.
            </p>
          </div>
        </div>

        <div className="about-section">
          <Image
            src={ronniePhoto}
            alt="Dr. Ronnie De Souza Santos Portrait"
            width={200}
            height={200}
            className="about-img"
          />
          <div>
            <h4>Dr. Ronnie de Souza Santos, Ph.D.</h4>
            <p>
              Dr. Ronnie de Souza Santos is an Assistant Professor in Software Engineering at the University of Calgary. 
              His research focuses on the human aspects of software engineering, software project management, 
              software quality and testing, fairness in software, and inclusive design practices. 
              Dr. Santos supports the development of tools that combine rigorous technical modeling 
              with human-centered decision-making.
            </p>
          </div>
        </div>

        <div className="info-modal-actions">
          <button className="close-button" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
}